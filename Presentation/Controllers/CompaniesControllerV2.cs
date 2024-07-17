using MediatorService.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace Presentation.Controllers
{
    [Route("api/companies")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    public class CompaniesControllerV2 : ControllerBase
    {
        private readonly ISender _sender;

        public CompaniesControllerV2(ISender sender) => _sender = sender;

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = 
                await _sender.Send(new GetCompaniesQuery(trackChanges : false));

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

