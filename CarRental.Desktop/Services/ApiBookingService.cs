using CarRental.Application.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Desktop.Services
{
    public class ApiBookingService : IBookingServiceClient
    {
        private readonly IApiClient _apiClient;

        public ApiBookingService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<BookingDto>> GetAllBookingsAsync()
        {
            // Use Reports endpoint as the main list endpoint
            var (result, error) = await _apiClient.GetRawAsync<List<BookingDto>>("api/Reports/bookings");
            return result ?? new List<BookingDto>();
        }

        public async Task<BookingDto?> GetBookingByIdAsync(long id)
        {
            var (result, error) = await _apiClient.GetRawAsync<BookingDto>($"api/Bookings/{id}");
            return result;
        }

        public async Task<BookingDto> CreateBookingAsync(CreateBookingDto booking)
        {
            var (result, error) = await _apiClient.PostAsync<CreateBookingDto, BookingDto>("api/Bookings", booking);
            return result ?? throw new System.Exception(error ?? "Failed to create booking");
        }

        public async Task<bool> UpdateBookingAsync(BookingDto booking)
        {
            // No generic update endpoint available in current API spec
            // Specific actions (Cancel, Return) should be used instead
            return await Task.FromResult(false);
        }

        public async Task<bool> CancelBookingAsync(long id)
        {
            // Use PUT endpoint for cancellation
            // Note: clientID query param is optional in spec, omitting for now
            var (success, error) = await _apiClient.PutRawAsync($"api/Bookings/{id}/cancel", new { });
            return success;
        }

        public async Task<bool> ConfirmBookingAsync(long id)
        {
            // Use PUT endpoint for confirmation
            var (success, error) = await _apiClient.PutRawAsync($"api/Bookings/{id}/confirm", new { });
            return success;
        }

        public async Task<bool> CompleteBookingAsync(long id, ReturnVehicleDto returnDto)
        {
            var success = await _apiClient.PutAsync($"api/Bookings/{id}/return", returnDto);
            return success;
        }

        public async Task<List<BookingDto>> GetActiveBookingsAsync()
        {
            // Fetch all via reports and filter client-side
            var bookings = await GetAllBookingsAsync();
            var activeStatuses = new[] { "Active", "Pending", "Confirmed" };
            return bookings.Where(b => !string.IsNullOrEmpty(b.Status) && activeStatuses.Contains(b.Status)).ToList();
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            var (result, error) = await _apiClient.GetRawAsync<DashboardDto>("api/Dashboard");
            return result ?? new DashboardDto();
        }
    }
}
