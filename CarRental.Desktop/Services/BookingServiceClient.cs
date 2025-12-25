using CarRental.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Desktop.Services
{
    public class BookingServiceClient : IBookingServiceClient
    {
        private readonly List<BookingDto> _mockBookings = new()
        {
            new BookingDto
            {
                Id = 1,
                ClientId = 101,
                VehicleId = 201,
                ClientName = "Jean Dupont",
                VehicleName = "Toyota Corolla 2023",
                StartDate = DateTime.Now.AddDays(-2),
                EndDate = DateTime.Now.AddDays(5),
                TotalAmount = 350.50m,
                PickUpLocation = "Agence Paris Centre",
                DropOffLocation = "Agence Paris Centre",
                Status = "Active",
                IsPaid = true,
                Notes = "Client fidèle"
            },
            new BookingDto
            {
                Id = 2,
                ClientId = 102,
                VehicleId = 202,
                ClientName = "Marie Martin",
                VehicleName = "Renault Clio 2022",
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(3),
                TotalAmount = 210.75m,
                PickUpLocation = "Agence Lyon",
                DropOffLocation = "Agence Lyon",
                Status = "Active",
                IsPaid = false,
                Notes = "Première location"
            },
            new BookingDto
            {
                Id = 3,
                ClientId = 103,
                VehicleId = 203,
                ClientName = "Pierre Bernard",
                VehicleName = "Peugeot 308 2024",
                StartDate = DateTime.Now.AddDays(-5),
                EndDate = DateTime.Now.AddDays(-1),
                TotalAmount = 450.00m,
                PickUpLocation = "Agence Marseille",
                DropOffLocation = "Agence Marseille",
                Status = "Completed",
                IsPaid = true,
                Notes = "Retour en retard"
            }
        };

        public Task<List<BookingDto>> GetAllBookingsAsync()
        {
            return Task.FromResult(_mockBookings);
        }

        public Task<BookingDto?> GetBookingByIdAsync(long id)
        {
            var booking = _mockBookings.FirstOrDefault(b => b.Id == id);
            return Task.FromResult(booking);
        }

        public Task<BookingDto> CreateBookingAsync(CreateBookingDto bookingDto)
        {
            var newBooking = new BookingDto
            {
                Id = _mockBookings.Any() ? _mockBookings.Max(b => b.Id) + 1 : 1,
                ClientId = bookingDto.ClientId,
                VehicleId = bookingDto.VehicleId,
                ClientName = "Nouveau Client", // Normalement récupéré depuis BDD
                VehicleName = "Nouveau Véhicule", // Normalement récupéré depuis BDD
                StartDate = bookingDto.StartDate,
                EndDate = bookingDto.EndDate,
                PickUpLocation = bookingDto.PickUpLocation,
                DropOffLocation = bookingDto.DropOffLocation,
                TotalAmount = CalculatePrice(bookingDto.StartDate, bookingDto.EndDate, 50m),
                Status = "Pending",
                IsPaid = false,
                Notes = bookingDto.Notes,
                CreatedAt = DateTime.Now
            };

            _mockBookings.Add(newBooking);
            return Task.FromResult(newBooking);
        }

        public Task<bool> UpdateBookingAsync(BookingDto booking)
        {
            var existingBooking = _mockBookings.FirstOrDefault(b => b.Id == booking.Id);
            if (existingBooking != null)
            {
                existingBooking.ClientId = booking.ClientId;
                existingBooking.VehicleId = booking.VehicleId;
                existingBooking.ClientName = booking.ClientName;
                existingBooking.VehicleName = booking.VehicleName;
                existingBooking.StartDate = booking.StartDate;
                existingBooking.EndDate = booking.EndDate;
                existingBooking.PickUpLocation = booking.PickUpLocation;
                existingBooking.DropOffLocation = booking.DropOffLocation;
                existingBooking.TotalAmount = booking.TotalAmount;
                existingBooking.Status = booking.Status;
                existingBooking.IsPaid = booking.IsPaid;
                existingBooking.Notes = booking.Notes;

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> CancelBookingAsync(long id)
        {
            var booking = _mockBookings.FirstOrDefault(b => b.Id == id);
            if (booking != null)
            {
                booking.Status = "Cancelled";
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> ConfirmBookingAsync(long id)
        {
            var booking = _mockBookings.FirstOrDefault(b => b.Id == id);
            if (booking != null && booking.Status == "Pending")
            {
                booking.Status = "Confirmed";
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> CompleteBookingAsync(long id, ReturnVehicleDto returnDto)
        {
            var booking = _mockBookings.FirstOrDefault(b => b.Id == id);
            if (booking != null && booking.Status == "Confirmed")
            {
                booking.Status = "Completed";
                if (!string.IsNullOrEmpty(returnDto.ConditionNotes))
                {
                    booking.Notes = string.IsNullOrEmpty(booking.Notes) 
                        ? returnDto.ConditionNotes 
                        : $"{booking.Notes}\n{returnDto.ConditionNotes}";
                }
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<List<BookingDto>> GetActiveBookingsAsync()
        {
            var activeStatuses = new[] { "Active", "Pending", "Confirmed" };
            var activeBookings = _mockBookings
                .Where(b => activeStatuses.Contains(b.Status))
                .ToList();

            return Task.FromResult(activeBookings);
        }

        public Task<DashboardDto> GetDashboardDataAsync()
        {
            var dashboard = new DashboardDto
            {
                TotalVehicles = 25,
                AvailableVehicles = 15,
                RentedVehicles = 8,
                InMaintenanceVehicles = 2,
                TotalClients = 42,
                ActiveBookings = _mockBookings.Count(b => b.Status == "Active"),
                TotalRevenue = _mockBookings.Where(b => b.IsPaid).Sum(b => b.TotalAmount),
                MonthlyRevenue = _mockBookings
                    .Where(b => b.CreatedAt >= DateTime.Now.AddMonths(-1) && b.IsPaid)
                    .Sum(b => b.TotalAmount),
                RecentBookings = _mockBookings
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(5)
                    .ToList()
            };

            return Task.FromResult(dashboard);
        }

        private decimal CalculatePrice(DateTime start, DateTime end, decimal dailyRate)
        {
            var days = (end - start).Days;
            return days <= 0 ? dailyRate : days * dailyRate;
        }
    }
}