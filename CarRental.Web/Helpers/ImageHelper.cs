namespace CarRental.Web.Helpers
{
    public static class ImageHelper
    {
        private const string DefaultApiBaseUrl = "http://localhost:5120";

        public static string GetFullImageUrl(string? relativePath, string? apiBaseUrl = null)
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            if (relativePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return relativePath;

            var baseUrl = apiBaseUrl ?? DefaultApiBaseUrl;
            return $"{baseUrl}{relativePath}";
        }
    }
}
