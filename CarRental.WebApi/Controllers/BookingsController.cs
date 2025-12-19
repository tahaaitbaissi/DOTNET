using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.WebApi.Controllers
{
    /// <summary>
    /// Booking management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(
            IBookingService bookingService,
            IVehicleService vehicleService,
            ILogger<BookingsController> logger)
        {
            _bookingService = bookingService;
            _vehicleService = vehicleService;
            _logger = logger;
        }

        /// <summary>
        /// Get a booking by ID
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Booking details</returns>
        /// <response code="200">Returns the booking</response>
        /// <response code="404">Booking not found</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingDto>> GetById(long id)
        {
            var result = await _bookingService.GetBookingByIdAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Booking Not Found",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Download booking confirmation PDF
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>PDF file</returns>
        [HttpGet("{id:long}/pdf")]
        public async Task<IActionResult> DownloadPdf(long id)
        {
            var result = await _bookingService.GetBookingPdfAsync(id);

            if (!result.IsSuccess)
            {
                 return NotFound(new ProblemDetails
                {
                    Title = "PDF Generation Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return File(result.Value, "application/pdf", $"booking_{id}.pdf");
        }

        /// <summary>
        /// Get all bookings for a specific client
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <returns>List of bookings</returns>
        /// <response code="200">Returns the list of bookings</response>
        /// <response code="404">Client not found</response>
        [HttpGet("client/{clientId:long}")]
        [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetByClient(long clientId)
        {
            var result = await _bookingService.GetClientBookingsAsync(clientId);

            if (!result.IsSuccess)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Client Not Found",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Create a new booking
        /// </summary>
        /// <param name="dto">Booking creation data</param>
        /// <returns>Created booking</returns>
        /// <response code="201">Booking created successfully</response>
        /// <response code="400">Validation error or vehicle not available</response>
        [HttpPost]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingDto dto)
        {
            _logger.LogInformation("Creating booking for client {ClientId}, vehicle {VehicleId}",
                dto.ClientId, dto.VehicleId);

            var result = await _bookingService.CreateBookingAsync(dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Booking creation failed: {Error}", result.Error);
                return BadRequest(new ProblemDetails
                {
                    Title = "Booking Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _logger.LogInformation("Booking created successfully with ID {BookingId}", result.Value.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Confirm a pending booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Confirmed booking</returns>
        /// <response code="200">Booking confirmed successfully</response>
        /// <response code="400">Cannot confirm booking</response>
        /// <response code="404">Booking not found</response>
        [HttpPut("{id:long}/confirm")]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingDto>> Confirm(long id)
        {
            _logger.LogInformation("Confirming booking {BookingId}", id);

            var result = await _bookingService.ConfirmBookingAsync(id);

            if (!result.IsSuccess)
            {
                if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Booking Not Found",
                        Detail = result.Error,
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return BadRequest(new ProblemDetails
                {
                    Title = "Confirmation Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _logger.LogInformation("Booking {BookingId} confirmed successfully", id);
            return Ok(result.Value);
        }

        /// <summary>
        /// Cancel an existing booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="clientId">Client ID (for verification)</param>
        /// <returns>Success status</returns>
        /// <response code="200">Booking cancelled successfully</response>
        /// <response code="400">Cannot cancel booking (policy violation)</response>
        /// <response code="404">Booking not found</response>
        [HttpPut("{id:long}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(long id, [FromQuery] long clientId)
        {
            _logger.LogInformation("Cancellation request for booking {BookingId} by client {ClientId}",
                id, clientId);

            var result = await _bookingService.CancelBookingAsync(id, clientId);

            if (!result.IsSuccess)
            {
                // Determine if it's a not found or business rule violation
                if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Booking Not Found",
                        Detail = result.Error,
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return BadRequest(new ProblemDetails
                {
                    Title = "Cancellation Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _logger.LogInformation("Booking {BookingId} cancelled successfully", id);
            return Ok(new { message = "Booking cancelled successfully" });
        }

        /// <summary>
        /// Return a vehicle and complete the booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="dto">Return information</param>
        /// <returns>Updated booking with final amount</returns>
        /// <response code="200">Vehicle returned successfully</response>
        /// <response code="400">Cannot return vehicle</response>
        /// <response code="404">Booking not found</response>
        [HttpPut("{id:long}/return")]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingDto>> ReturnVehicle(long id, [FromBody] ReturnVehicleDto dto)
        {
            // Ensure the DTO matches the booking ID
            if (dto.BookingId != id)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Booking ID in URL must match booking ID in request body",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _logger.LogInformation("Vehicle return for booking {BookingId}", id);

            var result = await _vehicleService.ReturnVehicleAsync(dto);

            if (!result.IsSuccess)
            {
                if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Booking Not Found",
                        Detail = result.Error,
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return BadRequest(new ProblemDetails
                {
                    Title = "Return Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _logger.LogInformation("Vehicle returned successfully for booking {BookingId}", id);
            return Ok(result.Value);
        }
    }
}
