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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PickUpLocation { get; set; }
        public string DropOffLocation { get; set; }
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

        /// <summary>
        /// Calculate the rental duration in days
        /// </summary>
        public int GetDurationInDays()
        {
            return (EndDate - StartDate).Days;
        }

        /// <summary>
        /// Check if the booking is overdue based on the current time
        /// </summary>
        public bool IsOverdue()
        {
            return Status == BookingStatus.Confirmed && DateTime.UtcNow > EndDate;
        }

        /// <summary>
        /// Calculate overdue fees based on daily rate
        /// </summary>
        public decimal CalculateOverdueFee(decimal dailyRate, decimal overdueMultiplier = 1.5m)
        {
            if (!IsOverdue()) return 0;
            
            var overdueDays = (DateTime.UtcNow - EndDate).Days;
            return overdueDays * dailyRate * overdueMultiplier;
        }

        /// <summary>
        /// Check if this booking overlaps with another date range
        /// </summary>
        public bool OverlapsWith(DateTime otherStart, DateTime otherEnd)
        {
            return StartDate < otherEnd && otherStart < EndDate;
        }
    }
}