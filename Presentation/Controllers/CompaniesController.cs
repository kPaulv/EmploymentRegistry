using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetCompanies()
        {
            var companies = _serviceManager.CompanyService.GetAllCompanies(trackChanges: false);

            return Ok(companies);
        }

        [HttpGet("{id:guid}", Name = "CompanyById")]     // api/companies/id
        public IActionResult GetCompany(Guid id)
        {
            var company = _serviceManager.CompanyService.GetCompany(id, trackChanges: false);
            return Ok(company);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public IActionResult GetCompanyCollection([ModelBinder(BinderType = 
            typeof(ArrayModelBinder))]IEnumerable<Guid> ids) 
        {
            var companies = _serviceManager.CompanyService
                                               .GetCompaniesByIds(ids, trackChanges: false);

            return Ok(companies);
        }

        [HttpPost]
        public IActionResult CreateCompany([FromBody] CompanyCreateDto companyInput)
        {
            if (companyInput == null)
                return BadRequest("Request failed. The input company model is empty.");

            var company = _serviceManager.CompanyService.CreateCompany(companyInput);

            return CreatedAtRoute("CompanyById", new { id = company.Id }, company);
        }

        [HttpPost]
        public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyCreateDto> 
            companyInputDtos)
        {
            var companyCollection = _serviceManager.CompanyService
                                                    .CreateCompanyCollection(companyInputDtos);

            return CreatedAtRoute("CompanyCollection", new { companyCollection.ids }, 
                                    companyCollection.companyOutputDtos);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteCompany(Guid companyId)
        {
            _serviceManager.CompanyService.DeleteCompany(companyId, trackChanges: false);

            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public IActionResult UpdateCompany(Guid companyId, 
                                            [FromBody]CompanyUpdateDto companyUpdateDto)
        {
            if (companyUpdateDto is null)
                return BadRequest("Request failed. Company update model is empty.");

            _serviceManager.CompanyService.UpdateCompany(companyId, 
                                                         companyUpdateDto, 
                                                         trackChanges: true);

            return NoContent();
        }
    }
}
