using System;
using System.Collections.Generic;

namespace CarRental.Application.DTOs
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
        public List<string> ImageUrls { get; set; } = new();
        
        // Computed property for display
        public string DisplayName => $"{Year} {Make} {Model}";
        public string? MainImage => ImageUrls?.Count > 0 ? ImageUrls[0] : null;
    }
}

