using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Interfaces
{
    // This is Unit of Work
    public interface IRepositoryManager
    {
        // Storages are a link for accessing the repositories from outer layers of program
        ICompanyRepository CompanyStorage { get; }
        IEmployeeRepository EmployeeStorage { get; }
        // DbContext.SaveChanges()
        Task SaveAsync();
    }
}
