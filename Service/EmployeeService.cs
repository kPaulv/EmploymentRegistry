using AutoMapper;
using Contracts.Interfaces;
using Entities.Entities;
using Entities.Exceptions;
using Entities.Exceptions.NotFound;
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

        public async Task<IEnumerable<EmployeeOutputDto>> GetEmployeesAsync(Guid companyId, bool trackChanges)
        {
            var company = await _repository.CompanyStorage.GetCompanyAsync(companyId, trackChanges);
            if (company == null)
                throw new CompanyNotFoundException(companyId);

            var employees = await _repository.EmployeeStorage.GetEmployeesAsync(companyId, trackChanges);

            return _mapper.Map<IEnumerable<EmployeeOutputDto>>(employees);
        }

        public async Task<EmployeeOutputDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) 
        {
            var company = await _repository.CompanyStorage.GetCompanyAsync(companyId, trackChanges);
            if (company == null)
                throw new CompanyNotFoundException(companyId);

            var employee = await _repository.EmployeeStorage.GetEmployeeAsync(companyId, id, trackChanges);
            if (employee == null)
                throw new EmployeeNotFoundException(id);

            return _mapper.Map<EmployeeOutputDto>(employee);
        }

        public async Task<EmployeeOutputDto> CreateEmployeeForCompanyAsync(Guid companyId, 
            EmployeeCreateDto employeeInputDto, bool trackChanges) 
        {
            var company = await _repository.CompanyStorage.GetCompanyAsync(companyId, trackChanges);
            if (company == null)
                throw new CompanyNotFoundException(companyId);

            var employee = _mapper.Map<Employee>(employeeInputDto);

            _repository.EmployeeStorage.CreateEmployeeForCompany(companyId, employee);
            await _repository.SaveAsync();

            var employeeOutputDto = _mapper.Map<EmployeeOutputDto>(employee);

            return employeeOutputDto;
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, 
            bool trackChanges)
        {
            var company = await _repository.CompanyStorage.GetCompanyAsync(companyId, 
                                                                 trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);

            var employee = await _repository.EmployeeStorage.GetEmployeeAsync(companyId, 
                                                                    employeeId, 
                                                                    trackChanges);
            if (employee is null)
                throw new EmployeeNotFoundException(employeeId);

            _repository.EmployeeStorage.DeleteEmployee(employee);
            await _repository.SaveAsync();
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid employeeId,
                                             EmployeeUpdateDto employeeUpdateDto,
                                             bool companyTrackChanges,
                                             bool employeeTrackChanges)
        {
            var company = await _repository.CompanyStorage.GetCompanyAsync(companyId, companyTrackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);

            // if employeetrackChanges == true changes to employee variable
            // will be tracked by DbContext
            var employee = await _repository.EmployeeStorage.GetEmployeeAsync(companyId,
                                                                              employeeId,
                                                                              employeeTrackChanges);
            if (employee is null)
                throw new EmployeeNotFoundException(employeeId);

            // update employee entity from DbContext via mapping
            _mapper.Map(employeeUpdateDto, employee);
            await _repository.SaveAsync();
        }

        public async Task<(EmployeeUpdateDto employeeToPatch, Employee employee)> 
            GetEmployeeForPatchAsync(Guid companyId, Guid employeeId, bool companyTrackChanges,
                                    bool employeeTrackChanges)
        {
            var company = await _repository.CompanyStorage.GetCompanyAsync(companyId, 
                                                                companyTrackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);

            var employee = await _repository.EmployeeStorage.GetEmployeeAsync(companyId,
                                                                   employeeId,
                                                                   employeeTrackChanges);
            if (employee is null)
                throw new EmployeeNotFoundException(employeeId);

            // for patch we return Update DTO (reverse mapping)
            var employeeToPatch = _mapper.Map<EmployeeUpdateDto>(employee);

            return (employeeToPatch, employee);
        }

        public async Task SaveChangesForPatchAsync(EmployeeUpdateDto employeePatched, Employee employee)
        {
            _mapper.Map(employeePatched, employee);
            await _repository.SaveAsync();
        }
    }
}
