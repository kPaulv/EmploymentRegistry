using MediatorService.Commands;
using MediatorService.Notifications;
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
        private readonly IPublisher _publisher;

        public CompaniesControllerV2(ISender sender, IPublisher publisher)
        {
            _sender = sender;
            _publisher = publisher;
        }

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetCompanies()
        {
            var companies =
                await _sender.Send(new GetCompaniesQuery(trackChanges: false));

            var companies_v2 = companies.Select(comp =>
                new CompanyOutputDto()
                {
                    Id = comp.Id,
                    Name = comp.Name + " sold.",
                    FullAddress = comp.FullAddress
                });

            return Ok(companies_v2);
        }

        [HttpGet("{id:guid}", Name = "GetCompanyById")]
        public async Task<IActionResult> GetCompanyById(Guid id)
        {
            var company =
                await _sender.Send(new GetCompanyQuery(id, trackChanges: false));

            return Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody]CompanyCreateDto
            companyCreateDto)
        {
            if (companyCreateDto is null)
                return BadRequest("Error! Passed object is empty.");

            var company =
                await _sender.Send(new CreateCompanyCommand(companyCreateDto));

            return CreatedAtRoute("GetCompanyById", new { Id = company.Id }, company);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyUpdateDto companyUpdateDto)
        {
            if (companyUpdateDto is null)
                return BadRequest("Error! Passed update data is empty.");

            await _sender.Send(new UpdateCompanyCommand(id, companyUpdateDto, TrackChanges: true));

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            //await _sender.Send(new DeleteCompanyCommand(id, TrackChanges: true));
            await _publisher.Publish(new CompanyDeletedNotification(id, TrackChanges: true));

            return NoContent();
        }
    }
}

