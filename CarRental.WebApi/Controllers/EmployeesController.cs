using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize(Roles = "Employee")] // Admin only
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
        {
            var result = await _employeeService.GetAllEmployeesAsync();
            return Ok(result.Value);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<EmployeeDto>> GetById(long id)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(id);
            if (!result.IsSuccess) return NotFound(result.Error);
            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> Create([FromBody] CreateEmployeeDto dto)
        {
            var result = await _employeeService.CreateEmployeeAsync(dto);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            if (!result.IsSuccess) return NotFound(result.Error);
            return Ok(new { message = "Employee deleted successfully" });
        }
    }
}
