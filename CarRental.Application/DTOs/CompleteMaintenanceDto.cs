using System;

namespace CarRental.Application.DTOs
{
    public class CompleteMaintenanceDto
    {
        public long MaintenanceId { get; set; }
        public decimal Cost { get; set; }
        public string Notes { get; set; }
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
    }
}
