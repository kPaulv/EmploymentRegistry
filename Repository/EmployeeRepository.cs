using Contracts.Interfaces;
using Entities.Entities;

namespace Repository
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext) {}
    }
}
