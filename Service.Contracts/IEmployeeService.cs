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

        EmployeeOutputDto CreateEmployeeForCompany(Guid companyId,
                                                    EmployeeCreateDto employeeInputDto,
                                                    bool trackChanges);

        void DeleteEmployeeForCompany(Guid companyId, Guid employeeId, bool trackChanges);

        void UpdateEmployeeForCompany(Guid companyId, Guid employeeId,
                                      EmployeeUpdateDto employeeUpdateDto,
                                      bool companyTrackChanges,
                                      bool employeeTrackChanges);
    }
}
