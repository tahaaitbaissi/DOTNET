using System.ComponentModel.DataAnnotations;

namespace CarRental.Application.DTOs
{
    /// <summary>
    /// Data transfer object for user registration
    /// </summary>
    public record RegisterDto(
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        string Email,
        
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        string Username,
        
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        string FullName,
        
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        string Password,
        
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        string Phone,
        
        string? Address = null,
        
        [Required(ErrorMessage = "Driver license is required")]
        string? DriverLicense = null,
        
        DateTime? LicenseExpiry = null);
}

