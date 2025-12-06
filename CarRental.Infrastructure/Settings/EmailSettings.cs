namespace CarRental.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for email service
    /// </summary>
    public class EmailSettings
    {
        public const string SectionName = "Email";

        /// <summary>
        /// SMTP server hostname (e.g., smtp.gmail.com)
        /// </summary>
        public string SmtpHost { get; set; } = "smtp.gmail.com";

        /// <summary>
        /// SMTP server port (587 for TLS, 465 for SSL)
        /// </summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>
        /// SMTP username for authentication
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// SMTP password for authentication
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Email address to send from
        /// </summary>
        public string FromEmail { get; set; } = "noreply@carrental.com";

        /// <summary>
        /// Display name for the sender
        /// </summary>
        public string FromName { get; set; } = "Car Rental";

        /// <summary>
        /// When true, emails are logged to console instead of being sent
        /// </summary>
        public bool UseDevelopmentMode { get; set; } = true;

        /// <summary>
        /// Use SSL/TLS for connection
        /// </summary>
        public bool UseSsl { get; set; } = true;
    }
}

