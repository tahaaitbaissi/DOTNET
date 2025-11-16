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

        // Navigation Properties
        public virtual Client Client { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}