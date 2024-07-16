using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Presentation.Controllers.Base;
using Presentation.Extensions;
using Presentation.ModelBinders;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers
{
    [Route("api/companies")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    public class CompaniesControllerV2 : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CompaniesControllerV2(IServiceManager serviceManager) => _serviceManager = serviceManager;

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetCompanies()
        {
            var companiesResponse = await
                _serviceManager.CompanyService.GetAllCompaniesAsync(trackChanges: false);

            var companies = companiesResponse.GetResult<IEnumerable<CompanyOutputDto>>();

            var companies_v2 = companies.Select(comp => 
                new CompanyOutputDto() { 
                    Id = comp.Id, 
                    Name = comp.Name + " sold.", 
                    FullAddress = comp.FullAddress
                });

            return Ok(companies_v2);
        }
    }
}

