using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        [HttpGet]
        [Route("~/api/employee/{id}/reportstructure")]
        public IActionResult GetEmployeeReportingStructure(String id) 
        {
            _logger.LogDebug($"Received employee reporting structure for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
            {
                return NotFound();
            }

            var reportingStructure = _employeeService.GetReportingStructure(employee);

            return Ok(reportingStructure);
        }

        [HttpPost]
        [Route("~/api/employee/compensation")]
        public IActionResult CreateEmployeeCompensation([FromBody]Compensation compensation)
        {
            _logger.LogDebug($"Received compensation create request for '{compensation.EmployeeId} {compensation.EmployeeId}'");

            var employee = _employeeService.GetById(compensation.EmployeeId);

            if (employee == null)
            {
                return NotFound();
            }

            _employeeService.CreateCompensation(compensation);

            return CreatedAtRoute("getEmployeeCompensationById", new { id = compensation.EmployeeId }, compensation);
        }


        [HttpGet(Name = "getEmployeeCompensationById")]
        [Route("~/api/employee/{id}/compensation")]
        public IActionResult GetEmployeeCompensation(String id)
        {
            _logger.LogDebug($"Received employee compensation request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
            {
                return NotFound();
            }

            var compensation = _employeeService.GetCompensationByEmployeeId(id);

            if (compensation == null)
            {
                return NotFound();
            }

            return Ok(compensation);
        }
    }
}
