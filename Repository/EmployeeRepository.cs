using Contracts.Interfaces;
using Entities.Entities;

namespace Repository
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext) {}

        public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges) =>
            ReadByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
            .OrderBy(e => e.Name).ToList();

        public Employee GetEmployee(Guid companyId, Guid id, bool trackChanges) =>
            ReadByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id),
            trackChanges)
            .SingleOrDefault();
    }
}
