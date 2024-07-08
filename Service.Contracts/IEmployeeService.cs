using Entities.Entities;
using Entities.LinkModels;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Dynamic;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
        Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync
            (Guid companyId, LinkParameters linkParams, bool trackChanges);

        Task<EmployeeOutputDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges);

        Task<EmployeeOutputDto> CreateEmployeeForCompanyAsync(Guid companyId, 
            EmployeeCreateDto employeeInputDto, bool trackChanges);

        Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges);

        Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid employeeId,
                                      EmployeeUpdateDto employeeUpdateDto,
                                      bool companyTrackChanges,
                                      bool employeeTrackChanges);

        Task<(EmployeeUpdateDto employeeToPatch, Employee employee)> 
            GetEmployeeForPatchAsync(Guid companyId, Guid employeeId, bool companyTrackChanges, 
                                    bool employeeTrackChanges);

        Task SaveChangesForPatchAsync(EmployeeUpdateDto employeePatched, Employee employee);
    }
}
