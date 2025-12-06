using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRental.WebApi.Controllers
{
    /// <summary>
    /// User profile self-service endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IClientService clientService, ILogger<ProfileController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user's profile
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientDto>> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var result = await _clientService.GetClientByUserIdAsync(userId);

            if (!result.IsSuccess)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Profile Not Found",
                    Detail = "No client profile found for this user.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Update current user's profile
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientDto>> UpdateMyProfile([FromBody] UpdateClientDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            // First get the client ID for this user
            var clientResult = await _clientService.GetClientByUserIdAsync(userId);
            if (!clientResult.IsSuccess)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Profile Not Found",
                    Detail = "No client profile found for this user.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            var updateResult = await _clientService.UpdateClientAsync(clientResult.Value!.Id, dto);

            if (!updateResult.IsSuccess)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Update Failed",
                    Detail = updateResult.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return Ok(updateResult.Value);
        }
    }
}
