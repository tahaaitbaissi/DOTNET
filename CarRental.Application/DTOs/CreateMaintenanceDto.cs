using System;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Application.DTOs
{
    public class CreateMaintenanceDto
    {
        [Required]
        public long VehicleId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public string Type { get; set; } // "Routine", "Repair", "Inspection"
        
        public string Details { get; set; }
    }
}
