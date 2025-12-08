using System.ComponentModel.DataAnnotations;

namespace CarRental.Application.DTOs
{
    public class UpdateVehicleTypeDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal BaseRate { get; set; }
    }
}
