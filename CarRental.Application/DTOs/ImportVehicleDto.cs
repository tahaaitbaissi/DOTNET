namespace CarRental.Application.DTOs
{
    public class ImportVehicleDto
    {
        public string VIN { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public string LicensePlate { get; set; }
        public string VehicleType { get; set; } // Matches available VehicleTypes
        public decimal PricePerDay { get; set; }
        public int Geometry { get; set; }
    }
}
