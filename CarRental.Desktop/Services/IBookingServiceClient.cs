using CarRental.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Desktop.Services
{
    public interface IBookingServiceClient
    {
        Task<List<BookingDto>> GetAllBookingsAsync();
        Task<BookingDto?> GetBookingByIdAsync(long id);
        Task<BookingDto> CreateBookingAsync(CreateBookingDto booking);
        Task<bool> UpdateBookingAsync(BookingDto booking);
        Task<bool> CancelBookingAsync(long id);
        Task<bool> ConfirmBookingAsync(long id);
        Task<bool> CompleteBookingAsync(long id, ReturnVehicleDto returnDto);
        Task<List<BookingDto>> GetActiveBookingsAsync();
        Task<DashboardDto> GetDashboardDataAsync();
    }
}