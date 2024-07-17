using AutoMapper;
using Contracts.Interfaces;
using Entities.ConfigModels;
using Entities.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Service.Contracts;

namespace Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICompanyService> _companyService;
        private readonly Lazy<IEmployeeService> _employeeService;
        private readonly Lazy<IAuthenticationService> _authenticationService;

        public ServiceManager(IRepositoryManager repository, 
                                ILoggerManager logger, 
                                IMapper mapper, 
                                IEmployeeLinks employeeLinks,
                                IOptionsSnapshot<JwtConfiguration> configuration, // for Authentication service
                                UserManager<User> userManager, // for Authentication service
                                RoleManager<IdentityRole> roleManager) // for Authentication service
        {
            // TODO: Implement HATEOAS for Companies (Service CompanyLinks)
            _companyService = new Lazy<ICompanyService>(() => 
                new CompanyService(repository, logger, mapper)
            );
            _employeeService = new Lazy<IEmployeeService>(() => 
                new EmployeeService(repository, logger, mapper, employeeLinks)
            );
            _authenticationService = new Lazy<IAuthenticationService>(() =>
                new AuthenticationService(mapper, configuration, logger, userManager, roleManager)
            );
        }

        public ICompanyService CompanyService => _companyService.Value;
        public IEmployeeService EmployeeService => _employeeService.Value;
        public IAuthenticationService AuthenticationService => _authenticationService.Value;

    }
}
