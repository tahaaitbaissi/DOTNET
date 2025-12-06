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
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        // Alert constants
        private const int MileageThreshold = 10000; // Alert every 10k km
        private const int MonthsThreshold = 6;     // Alert every 6 months

        public MaintenanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Maintenance>> ScheduleMaintenanceAsync(CreateMaintenanceDto dto)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(dto.VehicleId);
            if (vehicle == null) return Result<Maintenance>.Failure("Vehicle not found.");

            var maintenance = new Maintenance
            {
                VehicleId = dto.VehicleId,
                StartDate = dto.StartDate,
                Type = dto.Type,
                Details = dto.Details,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Update Vehicle Status
            vehicle.UpdateStatus(VehicleStatus.InMaintenance); 

            await _unitOfWork.Maintenances.AddAsync(maintenance);
            await _unitOfWork.Vehicles.UpdateAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();

            return Result<Maintenance>.Success(maintenance);
        }

        public async Task<Result<bool>> CompleteMaintenanceAsync(CompleteMaintenanceDto dto)
        {
            var maintenance = await _unitOfWork.Maintenances.GetByIdAsync(dto.MaintenanceId);
            if (maintenance == null) return Result<bool>.Failure("Maintenance record not found.");

            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(maintenance.VehicleId);
            
            maintenance.IsCompleted = true;
            maintenance.EndDate = dto.EndDate;
            maintenance.Cost = dto.Cost;
            maintenance.Details += $"\nCompletion Notes: {dto.Notes}";
            maintenance.UpdatedAt = DateTime.UtcNow;

            if (vehicle != null)
            {
                vehicle.UpdateStatus(VehicleStatus.Available);
                // Reset maintenance trackers
                vehicle.LastMaintenanceDate = dto.EndDate;
                vehicle.LastMaintenanceMileage = vehicle.CurrentMileage;
            }

            await _unitOfWork.Maintenances.UpdateAsync(maintenance);
            if(vehicle != null) await _unitOfWork.Vehicles.UpdateAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<Maintenance>>> GetActiveMaintenancesAsync()
        {
            var result = await _unitOfWork.Maintenances.GetActiveMaintenancesAsync();
            return Result<IEnumerable<Maintenance>>.Success(result);
        }

        public async Task<Result<int>> CheckAndGenerateAlertsAsync()
        {
            var vehicles = await _unitOfWork.Vehicles.GetAllAsync();
            var admins = await _unitOfWork.Employees.GetAllAsync(); // Naive: notify all admins/employees
            int alertsGenerated = 0;

            foreach (var v in vehicles)
            {
                bool needsMaintenance = false;
                string reason = string.Empty;

                // Check Mileage
                var lastMileage = v.LastMaintenanceMileage ?? 0;
                if ((v.CurrentMileage - lastMileage) >= MileageThreshold)
                {
                    needsMaintenance = true;
                    reason = $"Mileage exceeded threshold ({v.CurrentMileage - lastMileage} km since last service).";
                }

                // Check Time
                var lastDate = v.LastMaintenanceDate ?? v.CreatedAt; // Default to CreatedAt if no maintenance
                if ((DateTime.UtcNow - lastDate).TotalDays >= (30 * MonthsThreshold))
                {
                    needsMaintenance = true;
                    reason = string.IsNullOrEmpty(reason) 
                        ? $"Time exceeded ({MonthsThreshold} months since last service)." 
                        : reason + " Also time threshold exceeded.";
                }

                if (needsMaintenance && v.Status != VehicleStatus.InMaintenance)
                {
                    // Create Notification for each Admin
                    foreach (var admin in admins)
                    {
                        var notif = new Notification
                        {
                            UserId = admin.UserId,
                            Title = $"Maintenance Alert: {v.Make} {v.Model} ({v.LicensePlate})",
                            Body = $"Vehicle requires maintenance. Reason: {reason}",
                            Type = "MaintenanceAlert",
                            IsRead = false,
                            Status = NotificationStatus.Sent,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.Notifications.AddAsync(notif);
                    }
                    alertsGenerated++;
                }
            }

            if (alertsGenerated > 0)
                await _unitOfWork.SaveChangesAsync();

            return Result<int>.Success(alertsGenerated);
        }

        public async Task<Result<IEnumerable<Notification>>> GetAdminAlertsAsync()
        {
            // Just get alerts for the current user context? 
            // For simplicity, we assume the controller passes the UserId or we get generic alerts.
            // But interface implementation shouldn't depend on HttpContext here if possible.
            // Let's assume we return all Notifications of type 'MaintenanceAlert'.
            
            // To be proper, we should probably add GetByType to Repo.
            // For now, using generic logic assuming manageable dataset or strictly following user context pattern later.
            return Result<IEnumerable<Notification>>.Failure("User ID required for fetching specific alerts."); 
        }
    }
}
