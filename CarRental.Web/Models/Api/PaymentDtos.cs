using System.ComponentModel.DataAnnotations;

namespace CarRental.Web.Models.Api
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

    public class ProcessPaymentDto
    {
        [Required(ErrorMessage = "Booking ID is required")]
        public long BookingId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 100000, ErrorMessage = "Amount must be between 0.01 and 100,000")]
        [Display(Name = "Payment Amount")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "CreditCard";

        [Display(Name = "Card Number")]
        public string? CardNumber { get; set; }

        [Display(Name = "Expiry Month")]
        public string? ExpiryMonth { get; set; }

        [Display(Name = "Expiry Year")]
        public string? ExpiryYear { get; set; }

        [Display(Name = "CVV")]
        public string? CVV { get; set; }

        [Display(Name = "Cardholder Name")]
        [StringLength(100)]
        public string? CardholderName { get; set; }
    }

    public class PaymentResponseViewModel
    {
        public bool IsSuccess { get; set; }
        public PaymentDto? Payment { get; set; }
        public string? ErrorMessage { get; set; }
        public long BookingId { get; set; }
    }
}
