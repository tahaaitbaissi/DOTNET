using System;

namespace CarRental.Application.DTOs
{
    public class ClientDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string DriverLicense { get; set; } = string.Empty;
        public DateTime LicenseExpiry { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
