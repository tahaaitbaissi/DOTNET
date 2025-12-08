using System;

namespace CarRental.Application.DTOs
{
    public class PaymentDto
    {
        public long Id { get; set; }
        public long BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionRef { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
