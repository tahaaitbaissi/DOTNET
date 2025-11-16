using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Core.Entities
{
    public class Role : AuditableEntity
    {
        public UserRole RoleName { get; set; }
        public string Description { get; set; }

        // Navigation Properties
        public ICollection<User> Users { get; set; }
    }
}
