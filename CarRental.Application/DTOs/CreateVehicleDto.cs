using CarRental.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Application.DTOs
{
    public class CreateVehicleDto
    {
        [Required]
        [MaxLength(17)]
        public string VIN { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Make { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; }

        [MaxLength(50)]
        public string Color { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string LicensePlate { get; set; } = string.Empty;

        [Required]
        public VehicleStatus Status { get; set; } = VehicleStatus.Available;

        [Required]
        public decimal PricePerDay { get; set; }

        public bool IsInsured { get; set; }
        public string? InsurancePolicy { get; set; }
        
        public int Geometry { get; set; } // For number of seats/doors etc.
        public string? Issues { get; set; }

        public long? VehicleTypeId { get; set; }
    }
}
