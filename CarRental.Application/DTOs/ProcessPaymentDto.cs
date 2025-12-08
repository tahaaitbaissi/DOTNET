using System.ComponentModel.DataAnnotations;

namespace CarRental.Application.DTOs
{
    public class ProcessPaymentDto
    {
        [Required]
        public long BookingId { get; set; }

        [Required]
        [Range(0.01, 100000)]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } // "CreditCard", "PayPal", "Cash"

        // Optional card details for simulation
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string CVV { get; set; }
    }
}
