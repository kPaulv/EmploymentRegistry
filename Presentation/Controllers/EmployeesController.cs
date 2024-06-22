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
        public IActionResult GetEmployeesForCompany(Guid companyId)
        {
            var employees = _serviceManager.EmployeeService
                                           .GetEmployees(companyId, 
                                                         trackChanges : false);
            return Ok(employees);
        }

        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
        public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var employee = _serviceManager.EmployeeService
                                          .GetEmployee(companyId, 
                                                       id, 
                                                       trackChanges: false);
            return Ok(employee);
        }

        [HttpPost]
        public IActionResult CreateEmployeeForCompany(Guid companyId, 
            [FromBody] EmployeeCreateDto employeeInput)
        {
            if (employeeInput is null)
                return BadRequest("Request failed. Input employee body is empty.");

            var employeeOutput = _serviceManager.EmployeeService.CreateEmployeeForCompany(companyId, 
                employeeInput, trackChanges : false);

            return CreatedAtRoute("GetEmployeeForCompany", 
                new { companyId, id = employeeOutput.Id}, employeeOutput);
        }

        [HttpDelete("{employeeId:guid}")]
        public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid employeeId)
        {
            _serviceManager.EmployeeService.DeleteEmployeeForCompany(companyId,
                                                                     employeeId,
                                                                     trackChanges: false);

            return NoContent();
        }

        [HttpPut("{employeeId:guid}")]
        public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid employeeId,
            [FromBody]EmployeeUpdateDto employeeUpdateDto)
        {
            if (employeeUpdateDto is null)
                return BadRequest("Request failed. Employee update body is empty.");

            _serviceManager.EmployeeService
                            .UpdateEmployeeForCompany(companyId, 
                                                      employeeId,
                                                      employeeUpdateDto, 
                                                      companyTrackChanges: false, 
                                                      employeeTrackChanges: true);

            return NoContent();
        }

        [HttpPatch("{employeeId:guid}")]
        public IActionResult PartiallyUpdateEmployeeForCompany(Guid companyId, 
            Guid employeeId, [FromBody]JsonPatchDocument<EmployeeUpdateDto> patchDoc)
        {
            if (patchDoc is null)
                return BadRequest("Request failed. Patch body is empty.");

            // pair of type ( EmployeeUpdateDto employeeToPatch , Employee employeeEntity )
            var tuple = _serviceManager.EmployeeService
                                            .GetEmployeeForPatch(companyId,
                                                                 employeeId, 
                                                                 companyTrackChanges: false, 
                                                                 employeeTrackChanges: true);

            patchDoc.ApplyTo(tuple.employeeToPatch);

            _serviceManager.EmployeeService.SaveChangesForPatch(tuple.employeeToPatch,
                                                                        tuple.employee);

            return NoContent();
        }
    }
}
