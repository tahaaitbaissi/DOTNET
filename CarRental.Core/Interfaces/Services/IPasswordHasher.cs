namespace CarRental.Core.Interfaces.Services
{
    /// <summary>
    /// Service for securely hashing and verifying passwords
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes a plain text password using a secure algorithm
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <returns>The hashed password string</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a plain text password against a hashed password
        /// </summary>
        /// <param name="password">The plain text password to verify</param>
        /// <param name="hashedPassword">The stored hashed password</param>
        /// <returns>True if the password matches, false otherwise</returns>
        bool VerifyPassword(string password, string hashedPassword);
    }
}

