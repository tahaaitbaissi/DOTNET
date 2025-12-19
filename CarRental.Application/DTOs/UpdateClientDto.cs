using System;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Application.DTOs
{
    public class UpdateClientDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string DriverLicense { get; set; } = string.Empty;

        public DateTime LicenseExpiry { get; set; }
    }
}
