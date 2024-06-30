using Contracts.Interfaces;
using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace Repository
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

        public async Task<PagedList<Employee>> GetEmployeesAsync
            (Guid companyId, EmployeeRequestParameters employeeParams, bool trackChanges)
        {
            var employees = 
                await ReadByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
                        .OrderBy(e => e.Name)
                        .Skip((employeeParams.PageNumber - 1) * employeeParams.PageSize)
                        .Take(employeeParams.PageSize)
                        .ToListAsync();

            //return PagedList<Employee>.ToPagedList(employees, 
            //    employeeParams.PageNumber, employeeParams.PageSize);

            // Count via Database call is faster than ToPagedList() 
            var employeesAmount = await ReadByCondition(e =>
                e.CompanyId.Equals(companyId), trackChanges).CountAsync();

            return new PagedList<Employee>(employees,
                                            employeesAmount,
                                            employeeParams.PageNumber,
                                            employeeParams.PageSize);
        }
        

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>
            await ReadByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
                    .SingleOrDefaultAsync();

        public void CreateEmployeeForCompany(Guid companyId, Employee employee) {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee) => Delete(employee);
    }
}
