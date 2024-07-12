using AutoMapper;
using Contracts.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Service.Contracts;
using Shared.DataTransferObjects.AuthDTO;
using Entities.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;
using Entities.Exceptions.BadRequest;
using Entities.ConfigModels;
using Microsoft.Extensions.Options;

namespace Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly IOptionsSnapshot<JwtConfiguration> _configuration;
        private readonly ILoggerManager _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly JwtConfiguration _jwtConfiguration;

        private User? _user;

        public AuthenticationService(IMapper mapper,
                                      IOptionsSnapshot<JwtConfiguration> configuration,
                                      ILoggerManager logger,
                                      UserManager<User> userManager,
                                      RoleManager<IdentityRole> roleManager)
        {
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;

            _jwtConfiguration = _configuration.Value;
        }

        public async Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistrationDto)
        {
            var user = _mapper.Map<User>(userRegistrationDto);

            var result = await _userManager.CreateAsync(user, userRegistrationDto.Password);

            if (result.Succeeded)
            {
                foreach (var role in userRegistrationDto.Roles)
                {
                    if (await _roleManager.RoleExistsAsync(role))
                        _userManager.AddToRoleAsync(user, role);
                }
            }

            return result;
        }

        public async Task<bool> AuthenticateUser(UserAuthenticationDto userAuthenticationDto)
        {
            //var user = await _userManager.FindByNameAsync(userAuthenticationDto.UserName);
            _user = await _userManager.FindByNameAsync(userAuthenticationDto.UserName);

            if (_user is null)
            {
                _logger.Error("Authentication failed. Wrong User name.");
                return false;
            }

            var result =
                await _userManager.CheckPasswordAsync(_user, userAuthenticationDto.Password);

            if (!result)
                _logger.Error("Authentication failed. Wrong password.");

            return result;
        }

        public async Task<TokenDto> GenerateToken(bool populateExpirationTime)
        {
            // tokenOptions needed to Write Access Token
            var signingCredentials = GetSigningCredentials();
            var securityClaims = await GetSecurityClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, 
                                                        securityClaims);

            //generating Refresh Token
            var refreshToken = GenerateRefreshToken();
            _user.RefreshToken = refreshToken;

            // set Expiration time if needed(e.g. first time authenticating)
            if (populateExpirationTime)
                _user.RefreshTokenExpirationTime = DateTime.Now.AddDays(7);

            // save changes to Db(Refresh Token & its Expiration Time)
            await _userManager.UpdateAsync(_user);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new TokenDto(accessToken, refreshToken);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Environment.GetEnvironmentVariable("EMPREGAPP_SECRET");
            // if system variable is not defined, return exception
            if (key is null)
            {
                _logger.Error("The authentication Secret is not defined on server!");
            }

            var reverseKey = new string(key.ToCharArray().Reverse().ToArray());

            var secret = 
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key + reverseKey));

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetSecurityClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(_user);
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials,
                                                            List<Claim> securityClaims)
        {
            var tokenOptions = new JwtSecurityToken(
                    issuer: _jwtConfiguration.ValidIssuer,
                    audience: _jwtConfiguration.ValidAudience,
                    claims: securityClaims,
                    expires: DateTime.Now.AddMinutes(
                        Convert.ToDouble(_jwtConfiguration.ExpirationTime)
                        ),
                    signingCredentials: signingCredentials
                );

            return tokenOptions;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

            var user = await _userManager.FindByNameAsync(principal.Identity.Name);
            // if user doesn't exist, refreshToken is fake or refresh token has expired
            if (user == null || user.RefreshToken != tokenDto.RefreshToken ||
                                    user.RefreshTokenExpirationTime <= DateTime.Now)
                throw new RefreshTokenBadRequestException();

            _user = user;

            // we refresh access token before refresh token has expired
            return await GenerateToken(populateExpirationTime: false); 
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var issuerSecret = Environment.GetEnvironmentVariable("EMPREGAPP_SECRET");
            issuerSecret += new string(issuerSecret.ToCharArray().Reverse().ToArray());

            // server token params to compare with incoming token
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidIssuer = _jwtConfiguration.ValidIssuer,
                ValidAudience = _jwtConfiguration.ValidAudience,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerSecret))
            };

            // token handler to validate incoming token
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParams, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                                    StringComparison.InvariantCulture))
            {
                throw new SecurityTokenException("Current token is invalid.");
            }

            return principal;
        }
    }
}
