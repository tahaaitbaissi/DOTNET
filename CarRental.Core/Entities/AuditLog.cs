using System;
using System.Collections.Generic;

namespace CarRental.Core.Entities
{
    public class AuditLog : Entity
    {
        public DateTime Timestamp { get; set; }
        public string KeyValues { get; set; }
        public string Entity { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string PerformedBy { get; set; }
        public string TransactionId { get; set; }

        // Methods from diagram
        public Dictionary<string, object> GetChanges()
        {
            throw new NotImplementedException();
        }
    }
}