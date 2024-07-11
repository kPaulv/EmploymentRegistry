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

namespace Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILoggerManager _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private User? _user;

        public AuthenticationService(IMapper mapper,
                                      IConfiguration configuration,
                                      ILoggerManager logger,
                                      UserManager<User> userManager,
                                      RoleManager<IdentityRole> roleManager)
        {
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
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

        public async Task<string> GenerateToken()
        {
            var signingCredentials = GetSigningCredentials();
            var securityClaims = await GetSecurityClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, 
                                                        securityClaims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
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
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var tokenOptions = new JwtSecurityToken(
                    issuer: jwtSettings["ValidIssuer"],
                    audience: jwtSettings["ValidAudience"],
                    claims: securityClaims,
                    expires: DateTime.Now.AddMinutes(
                        Convert.ToDouble(jwtSettings["ExpirationTime"])
                        ),
                    signingCredentials: signingCredentials
                );

            return tokenOptions;
        }
    }
}
