using System;
using CarRental.Core.Enums;

namespace CarRental.Core.Entities
{
    public class Payment : AuditableEntity
    {
        public long BookingId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentIntentId { get; set; }
        public string TransactionRef { get; set; }
        public string Notes { get; set; }

        // Navigation Properties
        public virtual Booking Booking { get; set; }
    }
}