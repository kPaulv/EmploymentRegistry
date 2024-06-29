using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Presentation.ModelBinders;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CompaniesController(IServiceManager serviceManager) => _serviceManager = serviceManager;

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await 
                _serviceManager.CompanyService.GetAllCompaniesAsync(trackChanges: false);

            return Ok(companies);
        }

        [HttpGet("{id:guid}", Name = "CompanyById")]     // api/companies/id
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await 
                _serviceManager.CompanyService.GetCompanyAsync(id, trackChanges: false);

            return Ok(company);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = 
            typeof(ArrayModelBinder))]IEnumerable<Guid> ids) 
        {
            var companies = await 
                _serviceManager.CompanyService.GetCompaniesByIdsAsync(ids, trackChanges: false);

            return Ok(companies);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyCreateDto companyInputDto)
        {
            var company = await 
                _serviceManager.CompanyService.CreateCompanyAsync(companyInputDto);

            return CreatedAtRoute("CompanyById", new { id = company.Id }, company);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyCreateDto> 
            companyInputDtos)
        {
            var companyCollection = await 
                _serviceManager.CompanyService.CreateCompanyCollectionAsync(companyInputDtos);

            return CreatedAtRoute("CompanyCollection", new { companyCollection.ids }, 
                                    companyCollection.companyOutputDtos);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompany(Guid companyId)
        {
            await _serviceManager.CompanyService.DeleteCompanyAsync(companyId, trackChanges: false);

            return NoContent();
        }

        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid companyId, 
                                            [FromBody]CompanyUpdateDto companyUpdateDto)
        {
            await _serviceManager.CompanyService.UpdateCompanyAsync(companyId, 
                                                                    companyUpdateDto, 
                                                                    trackChanges: true);

            return NoContent();
        }
    }
}
