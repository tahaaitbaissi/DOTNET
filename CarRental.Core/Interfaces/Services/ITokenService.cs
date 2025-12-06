using CarRental.Core.Entities;

namespace CarRental.Core.Interfaces.Services
{
    /// <summary>
    /// Service for generating and validating JWT tokens
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT token for the specified user
        /// </summary>
        /// <param name="user">The user to generate the token for</param>
        /// <returns>The JWT token string</returns>
        string GenerateToken(User user);

        /// <summary>
        /// Validates a JWT token
        /// </summary>
        /// <param name="token">The token to validate</param>
        /// <returns>True if the token is valid, false otherwise</returns>
        bool ValidateToken(string token);
    }
}

