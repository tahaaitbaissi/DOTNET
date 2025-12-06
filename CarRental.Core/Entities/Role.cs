using System;
using System.Collections.Generic;
using CarRental.Core.Enums;

namespace CarRental.Core.Entities
{
    public class Role : AuditableEntity
    {
        public UserRole RoleName { get; set; }
        public string Description { get; set; } = string.Empty;
        
        // Navigation Properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
