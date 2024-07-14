using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Presentation.ModelBinders;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers
{
    [Route("api/companies")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class CompaniesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CompaniesController(IServiceManager serviceManager) => _serviceManager = serviceManager;

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, OPTIONS");

            return Ok();
        }

        /// <summary>
        /// Gets all Companies existing in DB
        /// </summary>
        /// <returns>List of Companies</returns>
        [HttpGet]
        [HttpHead]
        [Authorize]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await 
                _serviceManager.CompanyService.GetAllCompaniesAsync(trackChanges: false);

            return Ok(companies);
        }

        /// <summary>
        /// Returns a certain Company with provided Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>a Company instance</returns>
        [HttpGet("{id:guid}", Name = "CompanyById")]     // api/companies/id
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 90)]
        [HttpCacheValidation(MustRevalidate = false)]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await 
                _serviceManager.CompanyService.GetCompanyAsync(id, trackChanges: false);

            return Ok(company);
        }

        /// <summary>
        /// Returns a collection of company with provided Ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = 
            typeof(ArrayModelBinder))]IEnumerable<Guid> ids) 
        {
            var companies = await 
                _serviceManager.CompanyService.GetCompaniesByIdsAsync(ids, trackChanges: false);

            return Ok(companies);
        }

        /// <summary>
        /// Creates a new Company and puts it into DB
        /// </summary>
        /// <param name="companyInputDto"></param>
        /// <returns>Newly created Company instance</returns>
        /// <response code="201">Returns the newly created Company</response>
        /// <response code="400">If the item is null</response>
        /// <response code="201">If the model is invalid</response>
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyCreateDto companyInputDto)
        {
            var company = await 
                _serviceManager.CompanyService.CreateCompanyAsync(companyInputDto);

            return CreatedAtRoute("CompanyById", new { id = company.Id }, company);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [Route("CreateCompanyCollection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyCreateDto> 
            companyInputDtos)
        {
            var companyCollection = await 
                _serviceManager.CompanyService.CreateCompanyCollectionAsync(companyInputDtos);

            return CreatedAtRoute("CompanyCollection", new { companyCollection.ids }, 
                                    companyCollection.companyOutputDtos);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "SuperAdmin")]
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
