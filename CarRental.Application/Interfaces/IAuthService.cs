using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    /// <summary>
    /// Authentication service interface for user login and registration
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user with email and password
        /// </summary>
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);

        /// <summary>
        /// Registers a new user (client) account
        /// </summary>
        Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto);

        /// <summary>
        /// Validates if a token is still valid
        /// </summary>
        Task<Result<bool>> ValidateTokenAsync(string token);

        /// <summary>
        /// Verifies email with token
        /// </summary>
        Task<Result<bool>> VerifyEmailAsync(VerifyEmailDto dto);

        /// <summary>
        /// Initiates password reset process
        /// </summary>
        Task<Result<bool>> ForgotPasswordAsync(string email);

        /// <summary>
        /// Resets password with token
        /// </summary>
        Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto dto);
    }
}

