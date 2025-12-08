using System;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Application.DTOs
{
    public class CreateEmployeeDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Position { get; set; }
        
        public DateTime HireDate { get; set; } = DateTime.UtcNow;
    }
}
