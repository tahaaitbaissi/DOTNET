using System;
using System.IO;
using System.Text.Json;

namespace CarRental.Desktop.Configuration
{
    public class AppSettings
    {
        public string ApiBaseUrl { get; set; } = "http://localhost:5120"; // Default to local API
        public bool UseMockServices { get; set; } = false; // Default to real services
    }

    public static class ConfigurationLoader
    {
        public static AppSettings Load()
        {
            var configPath = "appsettings.json";
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<AppSettings>(json, options) ?? new AppSettings();
            }
            return new AppSettings();
        }
    }
}
