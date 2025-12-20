using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;

namespace CarRental.Desktop.Services
{
    public interface IPaymentService
    {
        Task<List<PaymentDto>> GetAllPaymentsAsync();
        Task ProcessPaymentAsync(ProcessPaymentDto dto);
    }
}
