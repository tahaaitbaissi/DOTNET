using System.ComponentModel.DataAnnotations;

namespace CarRental.Application.DTOs
{
    /// <summary>
    /// Data transfer object for user login
    /// </summary>
    public record LoginDto(
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        string Email,
        
        [Required(ErrorMessage = "Password is required")]
        string Password);
}

