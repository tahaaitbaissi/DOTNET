using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.WebApi.Controllers
{
    /// <summary>
    /// Authentication endpoints for user login and registration
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticate a user and receive a JWT token
        /// </summary>
        /// <param name="dto">Login credentials</param>
        /// <returns>JWT token and user information</returns>
        /// <response code="200">Returns the JWT token and user info</response>
        /// <response code="400">Invalid credentials or validation error</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

            var result = await _authService.LoginAsync(dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Login failed for email: {Email}", dto.Email);
                return BadRequest(new ProblemDetails
                {
                    Title = "Login Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _logger.LogInformation("User logged in successfully: {Email}", dto.Email);
            return Ok(result.Value);
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="dto">Registration information</param>
        /// <returns>JWT token and user information</returns>
        /// <response code="201">Account created successfully</response>
        /// <response code="400">Validation error or email/username already exists</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", dto.Email);

            var result = await _authService.RegisterAsync(dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Registration failed for email: {Email} - {Error}", dto.Email, result.Error);
                return BadRequest(new ProblemDetails
                {
                    Title = "Registration Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _logger.LogInformation("User registered successfully: {Email}", dto.Email);
            return CreatedAtAction(nameof(Login), result.Value);
        }

        /// <summary>
        /// Validate if a token is still valid
        /// </summary>
        /// <returns>Token validation result</returns>
        /// <response code="200">Token is valid</response>
        /// <response code="401">Token is invalid or expired</response>
        [HttpGet("validate")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ValidateToken()
        {
            // If we reach here, the token is valid (Authorize attribute passed)
            return Ok(new { valid = true, message = "Token is valid" });
        }

        /// <summary>
        /// Verify email address with code
        /// </summary>
        [HttpPost("verify")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
        {
            var result = await _authService.VerifyEmailAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Verification Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return Ok(new { message = "Email verified successfully" });
        }

        /// <summary>
        /// Request a password reset email
        /// </summary>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            // Always returns OK for security (don't reveal user existence)
            await _authService.ForgotPasswordAsync(dto.Email);
            return Ok(new { message = "If the account exists, a reset email has been sent." });
        }

        /// <summary>
        /// Reset password using token
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);

            if (!result.IsSuccess)
            {
                 return BadRequest(new ProblemDetails
                {
                    Title = "Reset Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return Ok(new { message = "Password has been reset successfully." });
        }
    }
}

