namespace CarRental.Application.DTOs
{
    /// <summary>
    /// DTO for changing password (authenticated user)
    /// </summary>
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
