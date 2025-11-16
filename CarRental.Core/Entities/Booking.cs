using System;
using CarRental.Core.Enums;

namespace CarRental.Core.Entities
{
    public class Booking : AuditableEntity
    {
        public long ClientId { get; set; }
        public long VehicleId { get; set; }

        public int? CreatedBy { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime Enable { get; set; }
        public string PickUpLocation { get; set; }
        public string DropOffLocation { get; set; }
        public DateTime BookingTime { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public string Notes { get; set; }

        // Navigation Properties
        public virtual Client Client { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Document> Documents { get; set; }

        public void Confirm()
        {
            Status = BookingStatus.Confirmed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            Status = BookingStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            Status = BookingStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        public decimal CalculateOverdue(DateTime endTime)
        {
            // TODO: Implement calculation
            throw new NotImplementedException();
        }

        public bool CheckOverdue(DateTime startDate, DateTime endDate)
        {
            // TODO: Implement check
            throw new NotImplementedException();
        }
    }
}