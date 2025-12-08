using System;

namespace CarRental.Desktop.Models
{
    public class Rental
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Active"; // Active, Completed, Cancelled
        public decimal TotalAmount { get; set; }
    }
}
