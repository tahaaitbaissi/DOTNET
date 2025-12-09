using System;
using System.Collections.Generic;

namespace CarRental.Core.Entities
{
    public class User : AuditableEntity
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; } = true;

        // Role FK (nullable)
        public long? RoleId { get; set; }
        public virtual Role? Role { get; set; }

        // Verification & Security
        public string? VerificationToken { get; set; }
        public DateTime? VerificationTokenExpiry { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        // Navigation Properties
        public virtual Client? Client { get; set; }
        public virtual Employee? Employee { get; set; }
        public virtual ICollection<Notification>? Notifications { get; set; }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}