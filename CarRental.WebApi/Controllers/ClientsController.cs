using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.WebApi.Controllers
{
    /// <summary>
    /// Client management endpoints (Admin only)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize(Roles = "Employee")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(IClientService clientService, ILogger<ClientsController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        /// <summary>
        /// Get all clients
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetAll()
        {
            var result = await _clientService.GetAllClientsAsync();
            return Ok(result.Value);
        }

        /// <summary>
        /// Get client by ID
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientDto>> GetById(long id)
        {
            var result = await _clientService.GetClientByIdAsync(id);

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
        /// Update client information
        /// </summary>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientDto>> Update(long id, [FromBody] UpdateClientDto dto)
        {
            var result = await _clientService.UpdateClientAsync(id, dto);

            if (!result.IsSuccess)
            {
                if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Client Not Found",
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
        /// Delete a client
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _clientService.DeleteClientAsync(id);

            if (!result.IsSuccess)
            {
                if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Client Not Found",
                        Detail = result.Error,
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return BadRequest(new ProblemDetails
                {
                    Title = "Deletion Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return Ok(new { message = "Client deleted successfully" });
        }
    }
}
