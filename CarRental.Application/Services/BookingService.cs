using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Core.Entities;
using CarRental.Core.Enums;
using CarRental.Core.Interfaces;
using CarRental.Core.Interfaces.Services;
using CarRental.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService? _emailService;
        private readonly IPdfService _pdfService;

        // Cancellation policy: minimum hours before start date to allow free cancellation
        private const int MinHoursBeforeCancellation = 24;

        public BookingService(IUnitOfWork unitOfWork, IPdfService pdfService, IEmailService? emailService = null)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _pdfService = pdfService;
        }

        public async Task<Result<BookingDto>> CreateBookingAsync(CreateBookingDto dto)
        {
            // Validate dates
            if (dto.StartDate >= dto.EndDate)
            {
                return Result<BookingDto>.Failure("Start date must be before end date.");
            }

            if (dto.StartDate < DateTime.UtcNow.Date)
            {
                return Result<BookingDto>.Failure("Start date cannot be in the past.");
            }

            // Check vehicle exists
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(dto.VehicleId);
            if (vehicle == null)
            {
                return Result<BookingDto>.Failure("Vehicle not found.");
            }

            if (vehicle.Status != VehicleStatus.Available)
            {
                return Result<BookingDto>.Failure("Vehicle is not available for booking.");
            }

            // Check client exists
            var client = await _unitOfWork.Clients.GetByIdAsync(dto.ClientId);
            if (client == null)
            {
                return Result<BookingDto>.Failure("Client not found.");
            }

            // Check vehicle availability for the date range
            var dateRange = new DateRange(dto.StartDate, dto.EndDate);
            var isAvailable = await _unitOfWork.Bookings.IsVehicleAvailableAsync(dto.VehicleId, dateRange);
            if (!isAvailable)
            {
                return Result<BookingDto>.Failure("Vehicle is not available for the selected dates.");
            }

            // Calculate total amount based on duration and tariffs
            var totalAmount = await CalculateTotalAmountAsync(vehicle, dateRange);

            // Create the booking
            var booking = new Booking
            {
                ClientId = dto.ClientId,
                VehicleId = dto.VehicleId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                PickUpLocation = dto.PickUpLocation,
                DropOffLocation = dto.DropOffLocation,
                Notes = dto.Notes ?? string.Empty,
                Status = BookingStatus.Pending,
                TotalAmount = totalAmount,
                IsPaid = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            // Send confirmation email (fire and forget, don't fail the booking if email fails)
            if (_emailService != null)
            {
                try
                {
                    await _emailService.SendBookingConfirmationAsync(booking);
                }
                catch
                {
                    // Log but don't fail
                }
            }

            return Result<BookingDto>.Success(MapToDto(booking, vehicle, client));
        }

        public async Task<Result<bool>> CancelBookingAsync(long bookingId, long clientId)
        {
            var bookings = await _unitOfWork.Bookings.GetBookingsForClientAsync(clientId);
            var booking = bookings.FirstOrDefault(b => b.Id == bookingId);

            if (booking == null)
            {
                return Result<bool>.Failure("Booking not found or does not belong to this client.");
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                return Result<bool>.Failure("Booking is already cancelled.");
            }

            if (booking.Status == BookingStatus.Completed)
            {
                return Result<bool>.Failure("Cannot cancel a completed booking.");
            }

            // Check cancellation policy: at least 24 hours before start date
            var hoursUntilStart = (booking.StartDate - DateTime.UtcNow).TotalHours;
            if (hoursUntilStart < MinHoursBeforeCancellation)
            {
                return Result<bool>.Failure($"Cancellation must be made at least {MinHoursBeforeCancellation} hours before the start date.");
            }

            // Cancel the booking
            booking.Cancel();
            await _unitOfWork.Bookings.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            // Send cancellation email
            if (_emailService != null)
            {
                try
                {
                    await _emailService.SendBookingCancelledAsync(booking);
                }
                catch
                {
                    // Log but don't fail
                }
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<BookingDto>>> GetClientBookingsAsync(long clientId)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
            if (client == null)
            {
                return Result<IEnumerable<BookingDto>>.Failure("Client not found.");
            }

            var bookings = await _unitOfWork.Bookings.GetBookingsForClientAsync(clientId);
            
            var dtos = bookings.Select(b => MapToDto(b, b.Vehicle, client)).ToList();
            
            return Result<IEnumerable<BookingDto>>.Success(dtos);
        }

        public async Task<Result<BookingDto>> GetBookingByIdAsync(long bookingId)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return Result<BookingDto>.Failure("Booking not found.");
            }

            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(booking.VehicleId);
            var client = await _unitOfWork.Clients.GetByIdAsync(booking.ClientId);

            return Result<BookingDto>.Success(MapToDto(booking, vehicle, client));
        }

        private async Task<decimal> CalculateTotalAmountAsync(Vehicle vehicle, DateRange dateRange)
        {
            // Simple calculation based on duration
            // In a real scenario, this would look up tariffs and apply complex pricing rules
            var durationDays = dateRange.DurationInDays();
            if (durationDays == 0) durationDays = 1; // Minimum 1 day

            // Get active tariff for this vehicle
            var tariff = vehicle.Tariffs?.FirstOrDefault(t => t.IsActive);
            
            if (tariff != null && tariff.PricePerDay.HasValue)
            {
                return tariff.PricePerDay.Value * durationDays;
            }

            // Fallback: use a default rate (this would normally come from configuration or vehicle type)
            const decimal defaultDailyRate = 50m;
            return defaultDailyRate * durationDays;
        }

        public async Task<Result<byte[]>> GetBookingPdfAsync(long bookingId)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return Result<byte[]>.Failure("Booking not found.");
            }

            // Ensure navigation properties are populated for the PDF
            if (booking.Vehicle == null)
            {
                booking.Vehicle = await _unitOfWork.Vehicles.GetByIdAsync(booking.VehicleId);
            }
            if (booking.Client == null)
            {
                booking.Client = await _unitOfWork.Clients.GetByIdAsync(booking.ClientId);
                if (booking.Client != null)
                {
                    // Ensure User is loaded for Client
                    var user = await _unitOfWork.Users.GetByIdAsync(booking.Client.UserId);
                    booking.Client.User = user;
                }
            }

            if (_pdfService == null)
            {
                return Result<byte[]>.Failure("PDF Service is not available.");
            }

            var pdfBytes = await _pdfService.GenerateBookingConfirmationPdfAsync(booking);
            return Result<byte[]>.Success(pdfBytes);
        }

        private static BookingDto MapToDto(Booking booking, Vehicle? vehicle, Client? client)
        {
            return new BookingDto
            {
                Id = booking.Id,
                VehicleId = booking.VehicleId,
                ClientId = booking.ClientId,
                VehicleName = vehicle != null ? $"{vehicle.Year} {vehicle.Make} {vehicle.Model}" : "Unknown",
                ClientName = client?.User?.FullName ?? "Unknown",
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                PickUpLocation = booking.PickUpLocation,
                DropOffLocation = booking.DropOffLocation,
                Status = booking.Status.ToString(),
                TotalAmount = booking.TotalAmount ?? 0,
                IsPaid = booking.IsPaid,
                Notes = booking.Notes,
                CreatedAt = booking.CreatedAt
            };
        }
    }
}

