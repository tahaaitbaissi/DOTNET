using System;

namespace CarRental.Core.Entities
{
    public class Maintenance : AuditableEntity
    {
        public long VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Type { get; set; }
        public string Details { get; set; }
        public bool IsCompleted { get; set; }
        public decimal? Cost { get; set; }

        // Navigation Properties
        public virtual Vehicle Vehicle { get; set; }

        // Methods from diagram
        public void CompleteMaintenance()
        {
            IsCompleted = true;
            EndDate = DateTime.UtcNow;
        }

        public void ExtendDuration(DateTime newEndDate)
        {
            EndDate = newEndDate;
        }
    }
}