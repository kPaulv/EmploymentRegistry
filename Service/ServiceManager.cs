using AutoMapper;
using Contracts.Interfaces;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICompanyService> _companyService;
        private readonly Lazy<IEmployeeService> _employeeService;

        public ServiceManager(IRepositoryManager repository, 
                                ILoggerManager logger, 
                                IMapper mapper, 
                                IEmployeeLinks employeeLinks)
        {
            // TODO: Implement HATEOAS for Companies (Service CompanyLinks)
            _companyService = new Lazy<ICompanyService>(() => 
                new CompanyService(repository, logger, mapper)
            );
            _employeeService = new Lazy<IEmployeeService>(() => 
                new EmployeeService(repository, logger, mapper, employeeLinks)
            );
        }

        public ICompanyService CompanyService => _companyService.Value;
        public IEmployeeService EmployeeService => _employeeService.Value;

    }
}
