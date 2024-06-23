using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

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
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId)
        {
            var employees = await 
                _serviceManager.EmployeeService.GetEmployeesAsync(companyId, 
                                                                    trackChanges : false);
            return Ok(employees);
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
        public async Task<IActionResult> CreateEmployeeForCompanyAsync(Guid companyId, 
            [FromBody] EmployeeCreateDto employeeInput)
        {
            if (employeeInput is null)
                return BadRequest("Request failed. Input employee body is empty.");

            // validation via attributes in DTO record
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var employeeOutput = await 
                _serviceManager.EmployeeService.CreateEmployeeForCompanyAsync(companyId, 
                                                                              employeeInput, 
                                                                              trackChanges : false);

            return CreatedAtRoute("GetEmployeeForCompany", 
                new { companyId, id = employeeOutput.Id}, employeeOutput);
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
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid employeeId,
            [FromBody]EmployeeUpdateDto employeeUpdateDto)
        {
            if (employeeUpdateDto is null)
                return BadRequest("Request failed. Employee update body is empty.");

            // update DTO validation
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

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
