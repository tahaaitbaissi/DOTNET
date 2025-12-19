namespace CarRental.Infrastructure.Settings
{
    /// <summary>
    /// General application settings
    /// </summary>
    public class AppSettings
    {
        public const string SectionName = "AppSettings";

        /// <summary>
        /// The base URL of the web application (for QR codes and links)
        /// </summary>
        public string WebAppUrl { get; set; } = "http://localhost:5235";
    }
}
