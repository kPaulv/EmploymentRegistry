using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
        IEnumerable<EmployeeOutputDto> GetEmployees(Guid companyId, bool trackChanges);

        EmployeeOutputDto GetEmployee(Guid companyId, Guid id, bool trackChanges);
    }
}
