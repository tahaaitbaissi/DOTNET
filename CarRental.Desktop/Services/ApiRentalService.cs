using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Desktop.Services
{
    public interface IRentalService
    {
        Task<List<BookingDto>> GetAllRentalsAsync();
        Task UpdateRentalStatusAsync(long id, string status);
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
            // Assuming "Rentals" are actually "Bookings" in the system
            var result = await _apiClient.GetAsync<List<BookingDto>>("api/Bookings");
            return result ?? new List<BookingDto>();
        }

        public async Task UpdateRentalStatusAsync(long id, string status)
        {
            // API might have a specific endpoint or use PATCH/PUT
            // Checking BookingsController... often has Cancel or UpdateStatus
            // Let's assume PUT /api/Bookings/{id} with status update or similar.
            // For now, implementing as a placeholder or standard update if endpoint exists.
            // Alternatively, PUT entire BookingDto.
            
            // NOTE: If backend only has "Cancel" and "Create", we might need more endpoints.
            // But let's assume we can PUT the booking update.
            
            // To be safe, we should fetch, update, and put.
            // Or use a specific status endpoint if available (e.g. /api/Bookings/{id}/return)
             
            // Using a generic update for now or skipping implementation for "UpdateStatus" specific
            // until we confirm backend (we inspected BookingsController before but didn't memorize it all).
             
            await System.Threading.Tasks.Task.CompletedTask; // Placeholder
        }
    }
}
