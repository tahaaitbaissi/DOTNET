using System;

namespace CarRental.Core.Entities
{
    public class Document : AuditableEntity
    {
        public long? BookingId { get; set; }
        public long? VehicleId { get; set; }
        public long? ClientId { get; set; }

        public string FilePath { get; set; }
        public string DocumentType { get; set; }
        public int? UploadedBy { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual Client Client { get; set; }
    }
}