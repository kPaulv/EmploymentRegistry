using AutoMapper;
using Contracts.Interfaces;
using Entities.Entities;
using Entities.Exceptions;
using Entities.Exceptions.BadRequest;
using Entities.Exceptions.NotFound;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

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

        // DRY - 2 helper methods to check if employee of company exists
        private async Task CheckCompanyExistsAsync(Guid companyId, bool trackChanges)
        {
            var company = await _repository.CompanyStorage.GetCompanyAsync(companyId, 
                                                                            trackChanges);

            if (company is null)
                throw new CompanyNotFoundException(companyId);
        }

        private async Task<Employee> GetEmployeeForCompanyIfExistsAsync
            (Guid companyId, Guid id, bool trackChanges)
        {
            var employee = await _repository.EmployeeStorage.GetEmployeeAsync(companyId, 
                                                                                id, trackChanges);

            if(employee is null)
                throw new EmployeeNotFoundException(id);

            return employee;
        }

        public async Task<(IEnumerable<EmployeeOutputDto> employeeDtos, MetaData metaData)> 
            GetEmployeesAsync
                (Guid companyId, EmployeeRequestParameters employeeParams, bool trackChanges)
        {
            if (!employeeParams.IsAgeRangeValid)
                throw new AgeRangeBadRequestException();

            await CheckCompanyExistsAsync(companyId, trackChanges);

            // get the PagedList of employees from Repository(certain page and amount of items on it)
            var employees = await _repository.EmployeeStorage.GetEmployeesAsync(companyId,
                                                                                employeeParams,         
                                                                                trackChanges);

            // map PageList of employees into list of Employee DTOs
            var employeeListDto = _mapper.Map<IEnumerable<EmployeeOutputDto>>(employees);

            // return tuple - (Employee DTOs , EmployeePageList.MetaData)
            return (employeeDtos: employeeListDto, metaData: employees.MetaData);
        }

        public async Task<EmployeeOutputDto> GetEmployeeAsync
            (Guid companyId, Guid id, bool trackChanges) 
        {
            await CheckCompanyExistsAsync(companyId, trackChanges);

            var employee = await GetEmployeeForCompanyIfExistsAsync(companyId, 
                                                                    id, 
                                                                    trackChanges);

            return _mapper.Map<EmployeeOutputDto>(employee);
        }

        public async Task<EmployeeOutputDto> CreateEmployeeForCompanyAsync(Guid companyId, 
            EmployeeCreateDto employeeInputDto, bool trackChanges) 
        {
            await CheckCompanyExistsAsync(companyId, trackChanges);

            var employee = _mapper.Map<Employee>(employeeInputDto);

            _repository.EmployeeStorage.CreateEmployeeForCompany(companyId, employee);
            await _repository.SaveAsync();

            var employeeOutputDto = _mapper.Map<EmployeeOutputDto>(employee);

            return employeeOutputDto;
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, 
            bool trackChanges)
        {
            await CheckCompanyExistsAsync(companyId, trackChanges);

            var employee = await GetEmployeeForCompanyIfExistsAsync(companyId, 
                                                                    employeeId, 
                                                                    trackChanges);

            _repository.EmployeeStorage.DeleteEmployee(employee);
            await _repository.SaveAsync();
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid employeeId,
                                             EmployeeUpdateDto employeeUpdateDto,
                                             bool companyTrackChanges,
                                             bool employeeTrackChanges)
        {
            await CheckCompanyExistsAsync(companyId, companyTrackChanges);

            // if employeetrackChanges == true changes to employee variable
            // will be tracked by DbContext
            var employee = await GetEmployeeForCompanyIfExistsAsync(companyId, 
                                                                    employeeId, 
                                                                    employeeTrackChanges);

            // update employee entity from DbContext via mapping
            _mapper.Map(employeeUpdateDto, employee);
            await _repository.SaveAsync();
        }

        public async Task<(EmployeeUpdateDto employeeToPatch, Employee employee)> 
            GetEmployeeForPatchAsync(Guid companyId, Guid employeeId, bool companyTrackChanges,
                                        bool employeeTrackChanges)
        {
            await CheckCompanyExistsAsync(companyId, companyTrackChanges);

            var employee = await GetEmployeeForCompanyIfExistsAsync(companyId,
                                                                    employeeId,
                                                                    employeeTrackChanges);

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
