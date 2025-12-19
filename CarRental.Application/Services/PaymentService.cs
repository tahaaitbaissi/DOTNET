using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Core.Entities;
using CarRental.Core.Enums;
using CarRental.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PaymentDto>> ProcessPaymentAsync(ProcessPaymentDto dto)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(dto.BookingId);
            if (booking == null)
            {
                return Result<PaymentDto>.Failure("Booking not found.");
            }

            if (booking.IsPaid)
            {
                return Result<PaymentDto>.Failure("Booking is already paid.");
            }

            // Simulate Payment Processing (e.g., call Stripe/PayPal here)
            // For MVP, we assume success if positive amount
            if (dto.Amount <= 0)
            {
                return Result<PaymentDto>.Failure("Invalid payment amount.");
            }

            // Ensure booking has required fields populated to avoid null errors
            if (string.IsNullOrWhiteSpace(booking.PickUpLocation))
            {
                booking.PickUpLocation = "Main Office";
            }
            if (string.IsNullOrWhiteSpace(booking.DropOffLocation))
            {
                booking.DropOffLocation = "Main Office";
            }
            if (booking.Notes == null)
            {
                booking.Notes = string.Empty;
            }

            var payment = new Payment
            {
                BookingId = dto.BookingId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod ?? "Cash",
                Status = PaymentStatus.Completed,
                TransactionRef = Guid.NewGuid().ToString("N"), // Simulated Ref
                PaymentIntentId = string.Empty,
                Notes = string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Payments.AddAsync(payment);

            // Update Booking Payment Status
            // Logic: Is the total amount covered?
            // For MVP simplicity, if they assume full payment:
            // Check if total payments >= booking total

            // Get all payments for this booking (including new one since context tracks it? No, AddAsync adds to context but query might not see it yet dependent on EF tracking)
            // Safest: Add current amount to existing paid sum.

            var existingPayments = await _unitOfWork.Payments.GetByBookingIdAsync(booking.Id);
            var totalPaid = existingPayments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount) + dto.Amount;

            if (totalPaid >= (booking.TotalAmount ?? 0))
            {
                booking.IsPaid = true;
                booking.Status = BookingStatus.Confirmed;
            }

            // Ensure all required fields are not null before updating
            booking.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Bookings.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            return Result<PaymentDto>.Success(MapToDto(payment));
        }

        public async Task<Result<PaymentDto>> GetPaymentByIdAsync(long id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null)
            {
                return Result<PaymentDto>.Failure("Payment not found.");
            }
            return Result<PaymentDto>.Success(MapToDto(payment));
        }

        public async Task<Result<IEnumerable<PaymentDto>>> GetPaymentsByBookingIdAsync(long bookingId)
        {
            var payments = await _unitOfWork.Payments.GetByBookingIdAsync(bookingId);
            var dtos = payments.Select(MapToDto).ToList();
            return Result<IEnumerable<PaymentDto>>.Success(dtos);
        }

        private static PaymentDto MapToDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                Status = payment.Status.ToString(),
                PaymentMethod = payment.PaymentMethod,
                TransactionRef = payment.TransactionRef,
                CreatedAt = payment.CreatedAt
            };
        }
    }
}
