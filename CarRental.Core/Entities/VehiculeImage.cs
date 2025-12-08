using System;

namespace CarRental.Core.Entities
{
    public class VehicleImage : AuditableEntity
    {
        public long VehicleId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public bool IsPrimary { get; set; }

        // Navigation Properties
        public virtual Vehicle Vehicle { get; set; }

        // Methods from diagram
        public string GetUrl()
        {
            throw new NotImplementedException();
        }
    }
}