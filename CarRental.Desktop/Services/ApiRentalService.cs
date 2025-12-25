using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Desktop.Services
{
    public interface IRentalService
    {
        Task<List<BookingDto>> GetAllRentalsAsync();
        Task DeleteRentalAsync(long id);
        Task CreateRentalAsync(CreateBookingDto rental);
        Task UpdateRentalAsync(long id, UpdateBookingDto rental);
        Task ConfirmRentalAsync(long id);
        Task CancelRentalAsync(long id);
        Task CompleteRentalAsync(long id, ReturnVehicleDto returnDto);
    }

    public class ApiRentalService : IRentalService
    {
        private readonly IApiClient _apiClient;

        public ApiRentalService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<BookingDto>> GetAllRentalsAsync()
        {
            var result = await _apiClient.GetAsync<List<BookingDto>>("api/bookings");
            return result ?? new List<BookingDto>();
        }

        public async Task CreateRentalAsync(CreateBookingDto rental)
        {
             await _apiClient.PostAsync("api/bookings", rental);
        }

        public async Task UpdateRentalAsync(long id, UpdateBookingDto rental)
        {
             await _apiClient.PutAsync($"api/bookings/{id}", rental);
        }

        public async Task DeleteRentalAsync(long id)
        {
             await _apiClient.DeleteAsync($"api/bookings/{id}");
        }

        public async Task ConfirmRentalAsync(long id)
        {
            await _apiClient.PutAsync($"api/bookings/{id}/confirm", new { });
        }

        public async Task CancelRentalAsync(long id)
        {
            await _apiClient.PutAsync($"api/bookings/{id}/cancel?clientId=0", new { });
        }

        public async Task CompleteRentalAsync(long id, ReturnVehicleDto returnDto)
        {
            await _apiClient.PutAsync($"api/bookings/{id}/return", returnDto);
        }
    }
}
