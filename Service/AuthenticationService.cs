using AutoMapper;
using Contracts.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Service.Contracts;
using Shared.DataTransferObjects.IdentityDTO;
using Entities.Entities;

namespace Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILoggerManager _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

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
                foreach(var role in userRegistrationDto.Roles)
                {
                    if (await _roleManager.RoleExistsAsync(role))
                        _userManager.AddToRoleAsync(user, role);
                }
            }

            return result;
        }
    }
}
