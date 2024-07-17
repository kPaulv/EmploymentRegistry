using Microsoft.AspNetCore.Http;
using Entities.LinkModels;
using Shared.DataTransferObjects;

namespace Contracts.Interfaces
{
    public interface IEmployeeLinks
    {
        LinkResponse TryGenerateLinks(IEnumerable<EmployeeOutputDto> employeesDto,
            Guid companyId, string fields, HttpContext httpContext);
    }
}
