using System;

namespace CarRental.Web.Models.Api
{
    public class BookingDto
    {
        public long Id { get; set; }
        public long VehicleId { get; set; }
        public string VehicleName { get; set; }
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // Pending, Confirmed, Cancelled, Completed
        public DateTime CreatedAt { get; set; }
    }

    public class CreateBookingDto
    {
        public long VehicleId { get; set; }
        public long ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    
    public class ReturnVehicleDto
    {
        public long BookingId { get; set; }
        public DateTime ReturnDate { get; set; }
        public int FinalMileage { get; set; }
        public bool IsDamaged { get; set; }
        public string DamageDescription { get; set; }
    }
}
