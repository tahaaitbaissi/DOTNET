using System.Collections.Generic;

namespace CarRental.Web.Models.Api
{
    public class VehicleDto
    {
        public long Id { get; set; }
        public string VIN { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Color { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsInsured { get; set; }
        public decimal DailyRate { get; set; }
        public string? VehicleTypeName { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();

        // Computed properties for display
        public string DisplayName => $"{Year} {Make} {Model}";
        public string? MainImage => ImageUrls?.Count > 0 ? ImageUrls[0] : null;

        // Additional properties for backward compatibility with existing views
        public int Seats { get; set; } = 5;
        public string Transmission { get; set; } = "Automatic";
        public string FuelType { get; set; } = "Petrol";
        public string ImageUrl => MainImage ?? string.Empty;
    }
}
