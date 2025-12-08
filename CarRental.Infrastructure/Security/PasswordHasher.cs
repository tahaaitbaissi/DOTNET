using CarRental.Core.Interfaces.Services;

namespace CarRental.Infrastructure.Security
{
    /// <summary>
    /// Password hashing service using BCrypt
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        // Work factor for BCrypt (higher = more secure but slower)
        // 11 is a good balance between security and performance
        private const int WorkFactor = 11;

        /// <summary>
        /// Hashes a password using BCrypt with automatic salt generation
        /// </summary>
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }

            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        /// <summary>
        /// Verifies a password against its hash using secure comparison
        /// </summary>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                // Invalid hash format - return false instead of throwing
                return false;
            }
        }
    }
}

