using System;

namespace CarRental.Application.DTOs
{
    public class BookingDto
    {
        public long Id { get; set; }
        public long VehicleId { get; set; }
        public long ClientId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PickUpLocation { get; set; } = string.Empty;
        public string DropOffLocation { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public string? Notes { get; set; }
        public int DurationInDays => (EndDate - StartDate).Days;
        public DateTime CreatedAt { get; set; }
    }
}