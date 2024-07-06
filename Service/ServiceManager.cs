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
                                IDataShaper<EmployeeOutputDto> employeeDataShaper,
                                IDataShaper<CompanyOutputDto> companyDataShaper)
        {
            _companyService = new Lazy<ICompanyService>(() => 
                new CompanyService(repository, logger, mapper, companyDataShaper)
            );
            _employeeService = new Lazy<IEmployeeService>(() => 
                new EmployeeService(repository, logger, mapper, employeeDataShaper)
            );
        }

        public ICompanyService CompanyService => _companyService.Value;
        public IEmployeeService EmployeeService => _employeeService.Value;

    }
}
