using System.ComponentModel.DataAnnotations;

namespace CarRental.Web.Models.Api
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        public string Phone { get; set; } = string.Empty; // Changed from PhoneNumber
        
        public string Address { get; set; } = string.Empty;
        
        [Required]
        public string DriverLicense { get; set; } = string.Empty; // Changed from DriverLicenseNumber
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public long Id { get; set; } // User ID
        public long? ClientId { get; set; } // If user is a client
    }
}
