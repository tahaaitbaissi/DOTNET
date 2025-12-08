using System;
using System.IO;
using System.Text.Json;

namespace CarRental.Desktop.Configuration
{
    public class AppSettings
    {
        public string ApiBaseUrl { get; set; } = "https://localhost:5001/api";
        public bool UseMockServices { get; set; } = true;
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
