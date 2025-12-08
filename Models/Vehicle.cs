namespace CarRental.Desktop.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public string Status { get; set; } = "Available"; // Available, Rented, Maintenance
        public decimal DailyRate { get; set; }
        public string ImagePath { get; set; } = string.Empty;
    }
}
