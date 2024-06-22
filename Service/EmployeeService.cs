﻿using AutoMapper;
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

        public EmployeeOutputDto CreateEmployeeForCompany(Guid companyId, 
            EmployeeInputDto employeeInputDto, bool trackChanges) 
        {
            var company = _repository.CompanyStorage.GetCompany(companyId, trackChanges);
            if (company == null)
                throw new CompanyNotFoundException(companyId);

            var employee = _mapper.Map<Employee>(employeeInputDto);

            _repository.EmployeeStorage.CreateEmployeeForCompany(companyId, employee);
            _repository.Save();

            var employeeOutputDto = _mapper.Map<EmployeeOutputDto>(employee);

            return employeeOutputDto;
        }

        public void DeleteEmployeeForCompany(Guid companyId, Guid employeeId, 
            bool trackChanges)
        {
            var company = _repository.CompanyStorage.GetCompany(companyId, 
                                                                 trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);

            var employee = _repository.EmployeeStorage.GetEmployee(companyId, 
                                                                    employeeId, 
                                                                    trackChanges);
            if (employee is null)
                throw new EmployeeNotFoundException(employeeId);

            _repository.EmployeeStorage.DeleteEmployee(employee);
            _repository.Save();
        }
    }
}
