using AutoMapper;
using Contracts.Interfaces;
using Entities.Exceptions;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public IEnumerable<EmployeeOutputDto> GetEmployees(Guid companyId, bool trackChanges)
        {
            var company = _repository.CompanyStorage.GetCompany(companyId, trackChanges);
            if (company == null)
                throw new CompanyNotFoundException(companyId);

            var employees = _repository.EmployeeStorage.GetEmployees(companyId, trackChanges);

            return _mapper.Map<IEnumerable<EmployeeOutputDto>>(employees);
        }

        public EmployeeOutputDto GetEmployee(Guid companyId, Guid id, bool trackChanges) 
        {
            var company = _repository.CompanyStorage.GetCompany(companyId, trackChanges);
            if (company == null)
                throw new CompanyNotFoundException(companyId);

            var employee = _repository.EmployeeStorage.GetEmployee(companyId, id, trackChanges);
            if (employee == null)
                throw new EmployeeNotFoundException(id);

            return _mapper.Map<EmployeeOutputDto>(employee);
        }
    }
}
