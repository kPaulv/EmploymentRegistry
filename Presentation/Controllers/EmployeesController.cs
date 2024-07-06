using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Presentation.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public EmployeesController(IServiceManager serviceManager) => 
            _serviceManager = serviceManager;

        [HttpGet]
        public async Task<IActionResult> GetEmployeesForCompany
            (Guid companyId, [FromQuery] EmployeeRequestParameters employeeRequestparams)
        {
            // get the tuple (employees , metadata{pageSize, pageCount})
            var employeesPagedTuple = await 
                _serviceManager.EmployeeService.GetEmployeesAsync(companyId,
                                                                  employeeRequestparams,
                                                                  trackChanges : false);

            Response.Headers.Add("X-Pagination", 
                                 JsonSerializer.Serialize(employeesPagedTuple.metaData));

            return Ok(employeesPagedTuple.employees);
        }

        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var employee = await 
                _serviceManager.EmployeeService.GetEmployeeAsync(companyId, id, 
                                                                    trackChanges: false);
            return Ok(employee);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, 
            [FromBody] EmployeeCreateDto employeeInputDto)
        {
            var employeeOutputDto = await 
                _serviceManager.EmployeeService.CreateEmployeeForCompanyAsync(companyId, 
                                                                              employeeInputDto, 
                                                                              trackChanges : false);

            return CreatedAtRoute("GetEmployeeForCompany", 
                new { companyId, id = employeeOutputDto.Id}, employeeOutputDto);
        }

        [HttpDelete("{employeeId:guid}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid employeeId)
        {
            await 
                _serviceManager.EmployeeService.DeleteEmployeeForCompanyAsync(companyId,
                                                                              employeeId,
                                                                              trackChanges: false);

            return NoContent();
        }

        [HttpPut("{employeeId:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid employeeId,
            [FromBody]EmployeeUpdateDto employeeUpdateDto)
        {
            await 
                _serviceManager.EmployeeService.UpdateEmployeeForCompanyAsync(companyId, 
                                                                              employeeId,
                                                                              employeeUpdateDto, 
                                                                              companyTrackChanges: false, 
                                                                              employeeTrackChanges: true);

            return NoContent();
        }

        [HttpPatch("{employeeId:guid}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, 
            Guid employeeId, [FromBody]JsonPatchDocument<EmployeeUpdateDto> patchDoc)
        {
            if (patchDoc is null)
                return BadRequest("Request failed. Patch body is empty.");

            // pair of type ( EmployeeUpdateDto employeeToPatch , Employee employeeEntity )
            var tuple = await 
                _serviceManager.EmployeeService.GetEmployeeForPatchAsync(companyId,
                                                                         employeeId, 
                                                                         companyTrackChanges: false, 
                                                                         employeeTrackChanges: true);

            // ApplyTo() applies PATCH operations to DTO and validates PATCH Doc
            patchDoc.ApplyTo(tuple.employeeToPatch, ModelState);

            // Validate modified DTO after applying PATCH operations
            TryValidateModel(tuple.employeeToPatch);

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            await _serviceManager.EmployeeService.SaveChangesForPatchAsync(tuple.employeeToPatch,
                                                                            tuple.employee);

            return NoContent();
        }
    }
}
