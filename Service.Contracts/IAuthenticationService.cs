using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects.AuthDTO;

namespace Service.Contracts
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistrationDto);
    }
}
