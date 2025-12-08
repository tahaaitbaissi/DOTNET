using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Core.Entities;
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
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService;

        public MaintenanceController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        [HttpPost("schedule")]
        public async Task<ActionResult<Maintenance>> Schedule([FromBody] CreateMaintenanceDto dto)
        {
            var result = await _maintenanceService.ScheduleMaintenanceAsync(dto);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpPost("complete")]
        public async Task<ActionResult<bool>> Complete([FromBody] CompleteMaintenanceDto dto)
        {
            var result = await _maintenanceService.CompleteMaintenanceAsync(dto);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(new { message = "Maintenance completed successfully" });
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Maintenance>>> GetActive()
        {
            var result = await _maintenanceService.GetActiveMaintenancesAsync();
            return Ok(result.Value);
        }

        [HttpPost("check-alerts")]
        public async Task<ActionResult<int>> CheckAlerts()
        {
            var result = await _maintenanceService.CheckAndGenerateAlertsAsync();
            return Ok(new { alertsGenerated = result.Value });
        }
    }
}
