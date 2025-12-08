namespace CarRental.Application.DTOs
{
    /// <summary>
    /// Response DTO for successful authentication
    /// </summary>
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserInfoDto User { get; set; } = null!;
    }

    /// <summary>
    /// Basic user information returned after authentication
    /// </summary>
    public class UserInfoDto
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public long? ClientId { get; set; }
        public long? EmployeeId { get; set; }
    }
}

