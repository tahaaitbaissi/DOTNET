using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<PaymentDto>> ProcessPaymentAsync(ProcessPaymentDto dto);
        Task<Result<PaymentDto>> GetPaymentByIdAsync(long id);
        Task<Result<IEnumerable<PaymentDto>>> GetPaymentsByBookingIdAsync(long bookingId);
        Task<Result<IEnumerable<PaymentDto>>> GetAllPaymentsAsync();
    }
}
