using AutoMapper;
using Contracts.Interfaces;
using Entities.Entities;
using Entities.Exceptions;
using Entities.Exceptions.BadRequest;
using Entities.Exceptions.NotFound;
using Entities.LinkModels;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Dynamic;

namespace Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IEmployeeLinks _employeeLinks;

        public EmployeeService(IRepositoryManager repository, 
                                ILoggerManager logger, 
                                IMapper mapper,
                                IEmployeeLinks employeeLinks)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _employeeLinks = employeeLinks;
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

        public async Task<(LinkResponse linkResponse, MetaData metaData)> 
            GetEmployeesAsync
                (Guid companyId, LinkParameters linkParams, bool trackChanges)
        {
            if (!linkParams.EmployeeParameters.IsAgeRangeValid)
                throw new AgeRangeBadRequestException();

            await CheckCompanyExistsAsync(companyId, trackChanges);

            // get the PagedList of employees from Repository(certain page and amount of items on it)
            var employees = await _repository.EmployeeStorage.GetEmployeesAsync(companyId, 
                linkParams.EmployeeParameters, trackChanges);

            // map PageList of employees into list of Employee DTOs
            var employeeListDto = _mapper.Map<IEnumerable<EmployeeOutputDto>>(employees);

            // shape mapped data (we try generation linked and shaped response, it will at least be shaped)
            var employeeLinkResponse = _employeeLinks.TryGenerateLinks(employeeListDto,
                companyId, linkParams.EmployeeParameters.Fields, linkParams.HttpContext);
            //var shapedEmployees = _dataShaper.ShapeData(employeeListDto, employeeParams.Fields);

            // return tuple - (Employee DTOs , EmployeePageList.MetaData)
            return (linkResponse: employeeLinkResponse, metaData: employees.MetaData);
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
