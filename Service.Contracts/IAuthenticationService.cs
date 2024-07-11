using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects.AuthDTO;

namespace Service.Contracts
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistrationDto);
        Task<bool> AuthenticateUser(UserAuthenticationDto userAuthenticationDto);
        Task<TokenDto> GenerateToken(bool populateExpirtionTime);
        Task<TokenDto> RefreshToken(TokenDto tokenDto);
    }
}
