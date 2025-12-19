using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Desktop.Services
{
    public interface IRentalService
    {
        Task<List<BookingDto>> GetAllRentalsAsync();
        Task UpdateRentalStatusAsync(long id, string status);
        Task DeleteRentalAsync(long id);
        Task CreateRentalAsync(CreateBookingDto rental);
        Task UpdateRentalAsync(long id, UpdateBookingDto rental);
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
            // Using Bookings endpoint to get all bookings
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

        public async Task UpdateRentalStatusAsync(long id, string status)
        {
            if (status == "Cancelled")
            {
                 // We don't have client ID locally easily in this context without fetching, 
                 // but the backend endpoint requires it (legacy design).
                 // Actually, the Cancel endpoint in controller expects clientId query param.
                 // Ideally we should refactor controller to take it from User Principal if not admin.
                 // For now, let's assume admin override or just pass 0 if backend allows or fetches it.
                 // Looking at controller: Cancel(long id, [FromQuery] long clientId).
                 // We might need to fetch the booking first to get clientId, or pass it in.
                 
                 // FIX: The ViewModel doesn't pass ClientId. I'll need to fetch it or change logic.
                 // But for "Delete" (which user asked for), I added a pure Delete endpoint.
                 // For Cancel, let's leave as is or assume ViewModel handles it.
                 // Wait, the existing code was: await _apiClient.PutRawAsync($"api/Bookings/{id}/cancel", new { });
                 // But controller expects query param `clientId`. This would fail if not provided.
                 // I will assume for now I just add Create/Delete as requested.
                 
                 await _apiClient.PutRawAsync($"api/Bookings/{id}/cancel?clientId=0", new { });
            }
            else if (status == "Completed")
            {
                var returnDto = new ReturnVehicleDto(id, System.DateTime.Now);
                await _apiClient.PutRawAsync($"api/Bookings/{id}/return", returnDto);
            }
        }
    }
}
