using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.WebApi.Controllers
{
    /// <summary>
    /// Vehicle management and search endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        /// <summary>
        /// Get all vehicles (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Employee")]
        [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAll()
        {
            var result = await _vehicleService.GetAllVehiclesAsync();
            return Ok(result.Value);
        }

        /// <summary>
        /// Create a new vehicle (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VehicleDto>> Create([FromBody] CreateVehicleDto dto)
        {
            var result = await _vehicleService.AddVehicleAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Create Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Update a vehicle (Admin only)
        /// </summary>
        [HttpPut("{id:long}")]
        [Authorize(Roles = "Employee")]
        [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VehicleDto>> Update(long id, [FromBody] UpdateVehicleDto dto)
        {
            var result = await _vehicleService.UpdateVehicleAsync(id, dto);

            if (!result.IsSuccess)
            {
                if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Vehicle Not Found",
                        Detail = result.Error,
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return BadRequest(new ProblemDetails
                {
                    Title = "Update Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Delete a vehicle (Admin only)
        /// </summary>
        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Employee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _vehicleService.DeleteVehicleAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Vehicle Not Found",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(new { message = "Vehicle deleted successfully" });
        }

        /// <summary>
        /// Upload an image for a vehicle (Admin only)
        /// </summary>
        [HttpPost("{id:long}/images")]
        [Authorize(Roles = "Employee")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> UploadImage(long id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ProblemDetails { Title = "Invalid File", Detail = "No file uploaded" });
            }

            using var stream = file.OpenReadStream();
            var result = await _vehicleService.AddVehicleImageAsync(id, stream, file.FileName);

            if (!result.IsSuccess)
            {
                if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new ProblemDetails { Title = "Not Found", Detail = result.Error });
                }
                return BadRequest(new ProblemDetails { Title = "Upload Failed", Detail = result.Error });
            }

            // Return full URL if possible, or relative path
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var fullUrl = $"{baseUrl}{result.Value}";

            return Ok(new { url = fullUrl, relativePath = result.Value });
        }

        /// <summary>
        /// Delete a vehicle image (Admin only)
        /// </summary>
        [HttpDelete("images/{imageId:long}")]
        [Authorize(Roles = "Employee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteImage(long imageId)
        {
            var result = await _vehicleService.RemoveVehicleImageAsync(imageId);

            if (!result.IsSuccess)
            {
                return NotFound(new ProblemDetails { Title = "Image Not Found", Detail = result.Error });
            }

            return Ok(new { message = "Image deleted successfully" });
        }

        /// <summary>
        /// Get a vehicle by ID
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <returns>Vehicle details</returns>
        /// <response code="200">Returns the vehicle</response>
        /// <response code="404">Vehicle not found</response>
        [HttpGet("{id:long}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VehicleDto>> GetById(long id)
        {
            var result = await _vehicleService.GetVehicleByIdAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Vehicle Not Found",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Search for available vehicles within a date range
        /// </summary>
        /// <param name="startDate">Rental start date</param>
        /// <param name="endDate">Rental end date</param>
        /// <param name="vehicleTypeId">Optional vehicle type filter</param>
        /// <param name="maxPricePerDay">Optional maximum price per day filter</param>
        /// <returns>List of available vehicles</returns>
        /// <response code="200">Returns the list of available vehicles</response>
        /// <response code="400">Invalid date range or parameters</response>
        [HttpGet("available")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAvailable(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] long? vehicleTypeId = null,
            [FromQuery] decimal? maxPricePerDay = null)
        {
            _logger.LogInformation(
                "Searching available vehicles from {StartDate} to {EndDate}, TypeId: {TypeId}, MaxPrice: {MaxPrice}",
                startDate, endDate, vehicleTypeId, maxPricePerDay);

            var searchDto = new VehicleSearchDto(startDate, endDate, vehicleTypeId, maxPricePerDay);
            var result = await _vehicleService.GetAvailableVehiclesAsync(searchDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Search Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _logger.LogInformation("Found {Count} available vehicles", 
                result.Value?.Count() ?? 0);
            
            return Ok(result.Value);
        }

        /// <summary>
        /// Search for available vehicles with POST body (for complex searches)
        /// </summary>
        /// <param name="search">Search criteria</param>
        /// <returns>List of available vehicles</returns>
        /// <response code="200">Returns the list of available vehicles</response>
        /// <response code="400">Invalid search parameters</response>
        [HttpPost("search")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> Search([FromBody] VehicleSearchDto search)
        {
            _logger.LogInformation(
                "Searching available vehicles from {StartDate} to {EndDate}",
                search.StartDate, search.EndDate);

            var result = await _vehicleService.GetAvailableVehiclesAsync(search);

            if (!result.IsSuccess)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Search Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return Ok(result.Value);
        }
    }
}

