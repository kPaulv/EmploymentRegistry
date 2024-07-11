using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects.IdentityDTO;

namespace Service.Contracts
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistrationDto);
    }
}
