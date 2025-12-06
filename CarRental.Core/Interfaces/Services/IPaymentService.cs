using CarRental.Core.Entities;
using System.Threading.Tasks;

namespace CarRental.Core.Interfaces.Services
{
    // A simple result object to return from the payment gateway
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string? TransactionId { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPaymentAsync(Payment payment);
        Task<PaymentResult> RefundPaymentAsync(Payment payment);
    }
}