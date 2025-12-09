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
            var (result, error) = await _apiClient.GetRawAsync<List<BookingDto>>("api/Bookings");
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
            var (success, error) = await _apiClient.PutRawAsync($"api/Bookings/{booking.Id}", booking);
            return success;
        }

        public async Task<bool> CancelBookingAsync(long id)
        {
            var (success, error) = await _apiClient.DeleteAsync($"api/Bookings/{id}");
            return success;
        }

        public async Task<List<BookingDto>> GetActiveBookingsAsync()
        {
            // First try a dedicated endpoint if backend provides it
            var (activeResult, activeError) = await _apiClient.GetRawAsync<List<BookingDto>>("api/Bookings/active");
            if (activeResult != null)
            {
                return activeResult;
            }

            // If not available or returned an error, fallback to fetching all and filtering client-side
            var (allResult, allError) = await _apiClient.GetRawAsync<List<BookingDto>>("api/Bookings");
            var bookings = allResult ?? new List<BookingDto>();

            var activeStatuses = new[] { "Active", "Pending", "Confirmed" };
            var filtered = bookings.Where(b => !string.IsNullOrEmpty(b.Status) && activeStatuses.Contains(b.Status)).ToList();
            return filtered;
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            var (result, error) = await _apiClient.GetRawAsync<DashboardDto>("api/Dashboard");
            return result ?? new DashboardDto();
        }
    }
}
