using CarRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CarRental.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Employee")] // Admin only
    public class ReportsController : ControllerBase
    {
        private readonly IExportService _exportService;
        private readonly IImportService _importService;

        public ReportsController(IExportService exportService, IImportService importService)
        {
            _exportService = exportService;
            _importService = importService;
        }

        [HttpGet("vehicles")]
        public async Task<IActionResult> ExportVehicles()
        {
            var fileContent = await _exportService.ExportVehiclesAsync();
            return File(fileContent, "text/csv", "vehicles_report.csv");
        }

        [HttpPost("import/vehicles")]
        public async Task<IActionResult> ImportVehicles(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!file.FileName.EndsWith(".csv"))
                return BadRequest("Only CSV files are supported.");

            using var stream = file.OpenReadStream();
            var result = await _importService.ImportVehiclesAsync(stream);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> ExportBookings()
        {
            var fileContent = await _exportService.ExportBookingsAsync();
            return File(fileContent, "text/csv", "bookings_report.csv");
        }

        [HttpGet("clients")]
        public async Task<IActionResult> ExportClients()
        {
            var fileContent = await _exportService.ExportClientsAsync();
            return File(fileContent, "text/csv", "clients_report.csv");
        }
    }
}
