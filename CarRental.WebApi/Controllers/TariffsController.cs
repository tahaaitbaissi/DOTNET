using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Employee")] // Admin only
    public class TariffsController : ControllerBase
    {
        private readonly ITariffService _tariffService;

        public TariffsController(ITariffService tariffService)
        {
            _tariffService = tariffService;
        }

        [HttpPost]
        public async Task<ActionResult<TariffDto>> CreateTariff([FromBody] CreateTariffDto dto)
        {
            var result = await _tariffService.AddTariffAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<ActionResult<IEnumerable<TariffDto>>> GetHistory(long vehicleId)
        {
            var result = await _tariffService.GetVehicleTariffHistoryAsync(vehicleId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }
    }
}
