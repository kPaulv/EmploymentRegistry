using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.AuthDTO;

namespace Presentation.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public AuthenticationController(IServiceManager serviceManager) => 
            _serviceManager = serviceManager;

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody]UserRegistrationDto userRegistrationDto)
        {
            var result = 
                await _serviceManager.AuthenticationService.RegisterUserAsync(userRegistrationDto);

            if(!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return StatusCode(201);
        }

        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserAuthenticationDto 
                                                                    userAuthenticationDto)
        {
            var authResult = 
                await _serviceManager.AuthenticationService
                                        .AuthenticateUser(userAuthenticationDto);
            if (!authResult)
                return Unauthorized();

            var token = 
                await _serviceManager.AuthenticationService
                                        .GenerateToken(userAuthenticationDto);

            // returning JWT in response body
            return token != null ? 
                        Ok(new { Token = token }) : 
                        StatusCode(500, new { Error = "Token generation error." }); 
        }

    }
}
