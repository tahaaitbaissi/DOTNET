namespace CarRental.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for JWT token generation
    /// </summary>
    public class JwtSettings
    {
        public const string SectionName = "Jwt";

        /// <summary>
        /// Secret key used to sign the JWT tokens (must be at least 32 characters)
        /// </summary>
        public string SecretKey { get; set; } = "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";

        /// <summary>
        /// Token issuer (usually your application name or URL)
        /// </summary>
        public string Issuer { get; set; } = "CarRental";

        /// <summary>
        /// Token audience (usually your application name or URL)
        /// </summary>
        public string Audience { get; set; } = "CarRental";

        /// <summary>
        /// Token expiration time in minutes
        /// </summary>
        public int ExpirationMinutes { get; set; } = 60;

        /// <summary>
        /// Refresh token expiration time in days
        /// </summary>
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}

