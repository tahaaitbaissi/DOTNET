using CarRental.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Application.DTOs
{
    public class UpdateVehicleDto
    {
        [Required]
        [MaxLength(50)]
        public string Color { get; set; } = string.Empty;

        [Required]
        public VehicleStatus Status { get; set; }

        public bool IsInsured { get; set; }
        public string? InsurancePolicy { get; set; }
        
        public string? Issues { get; set; }

        [Required]
        public decimal PricePerDay { get; set; }

        public long? VehicleTypeId { get; set; }
    }
}
