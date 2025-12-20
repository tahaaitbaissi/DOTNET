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
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DashboardDto>> GetDashboardDataAsync()
        {
            var dto = new DashboardDto();

            // 1. Vehicle Stats
            var allVehicles = await _unitOfWork.Vehicles.GetAllAsync();
            dto.TotalVehicles = allVehicles.Count();
            dto.AvailableVehicles = allVehicles.Count(v => v.Status == VehicleStatus.Available);
            dto.RentedVehicles = allVehicles.Count(v => v.Status == VehicleStatus.Rented);
            dto.InMaintenanceVehicles = allVehicles.Count(v => v.Status == VehicleStatus.InMaintenance);

            // 2. Client Stats
            dto.TotalClients = await _unitOfWork.Clients.CountAsync();

            // 3. Booking Stats
            var allBookings = await _unitOfWork.Bookings.GetAllAsync();
            dto.ActiveBookings = allBookings.Count(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending);

            // 4. Revenue
            // Naive approach: Sum all payments. Better approach: dedicated query but repo limited.
            // Using Payments repo directly if available.
            // We don't have GetAllPayments in Interface easily accessible without spec.
            // Actually Repository<T> has GetAllAsync.
            // Let's use Payments repo via UnitOfWork.Mapping might be needed? 
            // IUnitOfWork has Payments.
            var payments = await _unitOfWork.Payments.GetAllAsync();
            dto.TotalRevenue = payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
            
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            dto.MonthlyRevenue = payments
                .Where(p => p.Status == PaymentStatus.Completed && p.CreatedAt.Month == currentMonth && p.CreatedAt.Year == currentYear)
                .Sum(p => p.Amount);

            // 5. Recent Bookings (Last 5)
            // Using Linq on memory for now if GetAll returns IEnumerable. 
            // In huge DBs this is bad, but for this project scope it's standard MVP.
            var recent = allBookings
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    ClientId = b.ClientId,
                    VehicleId = b.VehicleId,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Status = b.Status.ToString(),
                    TotalAmount = b.TotalAmount ?? 0,
                    IsPaid = b.IsPaid,
                    CreatedAt = b.CreatedAt
                    // Note: ClientName/VehicleName might be missing here without Include. 
                    // Assume frontend handles it or we accept ID for dashboard.
                    // Or we fetch details.
                })
                .ToList();
            dto.RecentBookings = recent;

            // 6. Alerts
            var alerts = new List<string>();
            
            // Maintenance
            var maintenanceCount = allVehicles.Count(v => v.Status == VehicleStatus.InMaintenance);
            if (maintenanceCount > 0)
            {
                alerts.Add($"{maintenanceCount} vehicles currently in maintenance.");
            }

            // Overdue Returns (Active bookings where end date < now)
            var overdue = allBookings.Count(b => b.Status == BookingStatus.Confirmed && b.EndDate < DateTime.UtcNow);
            if (overdue > 0)
            {
                alerts.Add($"{overdue} bookings are overdue for return!");
            }

            // Returns Due Today
            var today = DateTime.UtcNow.Date;
            var returnsToday = allBookings.Count(b => b.Status == BookingStatus.Confirmed && b.EndDate.Date == today);
            if (returnsToday > 0)
            {
                alerts.Add($"{returnsToday} vehicles due for return today.");
            }

             // Pickups Due Today
            var pickupsToday = allBookings.Count(b => b.Status == BookingStatus.Confirmed && b.StartDate.Date == today);
            if (pickupsToday > 0)
             {
                 alerts.Add($"{pickupsToday} scheduled pickups for today.");
             }

            dto.Alerts = alerts;

            return Result<DashboardDto>.Success(dto);
        }
    }
}
