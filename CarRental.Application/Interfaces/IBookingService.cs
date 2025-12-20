using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IBookingService
    {
        /// <summary>
        /// Creates a new booking after validating dates and vehicle availability
        /// </summary>
        Task<Result<BookingDto>> CreateBookingAsync(CreateBookingDto dto);

        /// <summary>
        /// Confirms a pending booking
        /// </summary>
        Task<Result<BookingDto>> ConfirmBookingAsync(long bookingId);

        /// <summary>
        /// Cancels an existing booking if cancellation policy allows
        /// </summary>
        Task<Result<bool>> CancelBookingAsync(long bookingId, long clientId);

        /// <summary>
        /// Gets all bookings for a specific client
        /// </summary>
        Task<Result<IEnumerable<BookingDto>>> GetClientBookingsAsync(long clientId);

        /// <summary>
        /// Gets a specific booking by ID
        /// </summary>
        Task<Result<BookingDto>> GetBookingByIdAsync(long bookingId);

        /// <summary>
        /// Generates a PDF confirmation for the booking
        /// </summary>
        Task<Result<byte[]>> GetBookingPdfAsync(long bookingId);

        /// <summary>
        /// Gets all bookings in the system (for admin)
        /// </summary>
        Task<Result<IEnumerable<BookingDto>>> GetAllBookingsAsync();

        /// <summary>
        /// Deletes a booking (Admin only)
        /// </summary>
        Task<Result<bool>> DeleteBookingAsync(long bookingId);

        /// <summary>
        /// Updates an existing booking
        /// </summary>
        Task<Result<BookingDto>> UpdateBookingAsync(long bookingId, UpdateBookingDto dto);
    }
}
