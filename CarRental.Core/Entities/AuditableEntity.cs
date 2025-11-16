using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Core.Entities
{
    public class AuditableEntity : Entity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
