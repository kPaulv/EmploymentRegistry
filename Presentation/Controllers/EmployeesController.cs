using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            [FromBody] EmployeeInputDto employeeInput)
        {
            if (employeeInput is null)
                return BadRequest("Request failed. Input employee model is empty.");

            var employeeOutput = _serviceManager.EmployeeService.CreateEmployeeForCompany(companyId, 
                employeeInput, trackChanges : false);

            return CreatedAtRoute("GetEmployeeForCompany", 
                new { companyId, id = employeeOutput.Id}, employeeOutput);
        }
    }
}
