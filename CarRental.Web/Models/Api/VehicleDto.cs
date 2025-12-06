using System.Text.Json.Serialization;

namespace CarRental.Web.Models.Api
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Color { get; set; } = string.Empty;
        public decimal DailyRate { get; set; } // This might need to come from a Tariff or be calculated
        public string LicensePlate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "Available", "Rented", etc.
        public string ImageUrl { get; set; } = string.Empty; // URL to the image
        public int Seats { get; set; } = 4; // Default if not in backend entity yet
        public string Transmission { get; set; } = "Automatic"; // Default if not in backend entity yet
        public string FuelType { get; set; } = "Petrol";
    }
}
