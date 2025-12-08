using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.WebApi.Controllers
{
    [ApiController]
    [Route("api/vehicle-types")]
    [Produces("application/json")]
    [Authorize(Roles = "Employee")] // Admin access only
    public class VehicleTypesController : ControllerBase
    {
        private readonly IVehicleTypeService _vehicleTypeService;
        private readonly ILogger<VehicleTypesController> _logger;

        public VehicleTypesController(IVehicleTypeService vehicleTypeService, ILogger<VehicleTypesController> logger)
        {
            _vehicleTypeService = vehicleTypeService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VehicleTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VehicleTypeDto>>> GetAll()
        {
            var result = await _vehicleTypeService.GetAllVehicleTypesAsync();
            return Ok(result.Value);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(VehicleTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VehicleTypeDto>> GetById(long id)
        {
            var result = await _vehicleTypeService.GetVehicleTypeByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = result.Error });
            }
            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(VehicleTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VehicleTypeDto>> Create([FromBody] CreateVehicleTypeDto dto)
        {
            var result = await _vehicleTypeService.AddVehicleTypeAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(new ProblemDetails { Title = "Creation Failed", Detail = result.Error });
            }
            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
        }

        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(VehicleTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VehicleTypeDto>> Update(long id, [FromBody] UpdateVehicleTypeDto dto)
        {
            var result = await _vehicleTypeService.UpdateVehicleTypeAsync(id, dto);
            if (!result.IsSuccess)
            {
                if (result.Error!.Contains("not found", System.StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new ProblemDetails { Title = "Not Found", Detail = result.Error });
                }
                return BadRequest(new ProblemDetails { Title = "Update Failed", Detail = result.Error });
            }
            return Ok(result.Value);
        }

        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _vehicleTypeService.DeleteVehicleTypeAsync(id);
            if (!result.IsSuccess)
            {
                if (result.Error!.Contains("not found", System.StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new ProblemDetails { Title = "Not Found", Detail = result.Error });
                }
                return BadRequest(new ProblemDetails { Title = "Deletion Failed", Detail = result.Error });
            }
            return Ok(new { message = "Vehicle Type deleted successfully" });
        }
    }
}
