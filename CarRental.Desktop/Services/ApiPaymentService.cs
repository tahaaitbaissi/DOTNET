using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Desktop.Services
{
    public class ApiPaymentService : IPaymentService
    {
        private readonly IApiClient _apiClient;

        public ApiPaymentService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<PaymentDto>> GetAllPaymentsAsync()
        {
            // GET /api/Payments
            var result = await _apiClient.GetAsync<List<PaymentDto>>("api/Payments");
            return result ?? new List<PaymentDto>();
        }

        public async Task ProcessPaymentAsync(ProcessPaymentDto dto)
        {
            // POST /api/Payments/process
            await _apiClient.PostAsync("api/Payments/process", dto);
        }
    }
}
