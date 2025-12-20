using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Core.Entities;
using CarRental.Core.Enums;
using CarRental.Core.Interfaces;
using CarRental.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        // Overdue fee multiplier (1.5x normal daily rate)
        private const decimal OverdueMultiplier = 1.5m;

        public VehicleService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        // ... existing methods ...

        public async Task<Result<string>> AddVehicleImageAsync(long vehicleId, Stream fileStream, string fileName)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(vehicleId);
            if (vehicle == null)
            {
                return Result<string>.Failure("Vehicle not found.");
            }

            var uploadResult = await _fileService.UploadFileAsync(fileStream, fileName, "vehicles");
            if (!uploadResult.IsSuccess)
            {
                return Result<string>.Failure(uploadResult.Error);
            }

            var image = new VehicleImage
            {
                VehicleId = vehicleId,
                FilePath = uploadResult.Value,
                FileName = fileName,
                MimeType = "image/" + Path.GetExtension(fileName).TrimStart('.'),
                IsPrimary = false, // Could add logic for this
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.VehicleImages.AddAsync(image);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success(uploadResult.Value);
        }

        public async Task<Result<bool>> RemoveVehicleImageAsync(long imageId)
        {
            var image = await _unitOfWork.VehicleImages.GetByIdAsync(imageId);
            if (image == null)
            {
                return Result<bool>.Failure("Image not found.");
            }

            await _fileService.DeleteFileAsync(image.FilePath);
            
            await _unitOfWork.VehicleImages.DeleteAsync(image);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        // ... rest of the file ... (Wait, I should only replace specific parts or use multi_replace for constructor and methods)


        public async Task<Result<VehicleDto>> GetVehicleByIdAsync(long vehicleId)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(vehicleId);
            if (vehicle == null)
            {
                return Result<VehicleDto>.Failure("Vehicle not found.");
            }

            return Result<VehicleDto>.Success(MapToDto(vehicle));
        }


        public async Task<Result<IEnumerable<VehicleDto>>> GetAllVehiclesAsync()
        {
            var vehicles = await _unitOfWork.Vehicles.GetAllAsync();
            var dtos = vehicles.Select(MapToDto).ToList();
            return Result<IEnumerable<VehicleDto>>.Success(dtos);
        }

        public async Task<Result<VehicleDto>> AddVehicleAsync(CreateVehicleDto dto)
        {
            // Optional: Check if VIN already exists
            // var existing = await _unitOfWork.Vehicles.GetByVinAsync(dto.VIN);

            var vehicle = new Vehicle
            {
                VIN = dto.VIN,
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                Color = dto.Color,
                LicensePlate = dto.LicensePlate,
                Status = dto.Status,
                IsInsured = dto.IsInsured,
                InsurancePolicy = dto.InsurancePolicy ?? string.Empty,
                Geometry = dto.Geometry,
                Issues = dto.Issues ?? string.Empty,
                VehicleTypeId = dto.VehicleTypeId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Create initial tariff
            var tariff = new Tariff
            {
                PricePerDay = dto.PricePerDay,
                IsActive = true,
                StartDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            vehicle.Tariffs = new List<Tariff> { tariff };

            await _unitOfWork.Vehicles.AddAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();

            return Result<VehicleDto>.Success(MapToDto(vehicle));
        }

        public async Task<Result<VehicleDto>> UpdateVehicleAsync(long id, UpdateVehicleDto dto)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
            if (vehicle == null)
            {
                return Result<VehicleDto>.Failure("Vehicle not found.");
            }

            vehicle.Make = dto.Make;
            vehicle.Model = dto.Model;
            vehicle.Year = dto.Year;
            vehicle.LicensePlate = dto.LicensePlate;
            vehicle.Color = dto.Color;
            vehicle.Status = dto.Status;
            vehicle.IsInsured = dto.IsInsured;
            vehicle.InsurancePolicy = dto.InsurancePolicy ?? vehicle.InsurancePolicy;
            vehicle.Issues = dto.Issues ?? vehicle.Issues;
            vehicle.VehicleTypeId = dto.VehicleTypeId;
            vehicle.UpdatedAt = DateTime.UtcNow;

            // Handle Tariff update if price changed
            var currentTariff = vehicle.Tariffs?.FirstOrDefault(t => t.IsActive);
            if (currentTariff == null || currentTariff.PricePerDay != dto.PricePerDay)
            {
                // Deactivate old tariff
                if (currentTariff != null)
                {
                    currentTariff.IsActive = false;
                    currentTariff.EffectiveTo = DateTime.UtcNow;
                }

                // Add new tariff
                if (vehicle.Tariffs == null) vehicle.Tariffs = new List<Tariff>();
                
                vehicle.Tariffs.Add(new Tariff
                {
                    PricePerDay = dto.PricePerDay,
                    IsActive = true,
                    StartDate = DateTime.UtcNow,
                    VehicleId = vehicle.Id, 
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await _unitOfWork.Vehicles.UpdateAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();

            return Result<VehicleDto>.Success(MapToDto(vehicle));
        }

        public async Task<Result<bool>> DeleteVehicleAsync(long id)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
            if (vehicle == null)
            {
                return Result<bool>.Failure("Vehicle not found.");
            }

            // Check if vehicle has active bookings? 
            // For now, let's assume we can soft delete or just delete if no FK constraints
            // But usually we check dependencies. Repository.DeleteAsync handles standard delete to DB.

            await _unitOfWork.Vehicles.DeleteAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<VehicleDto>>> GetAvailableVehiclesAsync(VehicleSearchDto search)
        {
            // Validate date range
            if (search.StartDate >= search.EndDate)
            {
                return Result<IEnumerable<VehicleDto>>.Failure("Start date must be before end date.");
            }

            if (search.StartDate < DateTime.UtcNow.Date)
            {
                return Result<IEnumerable<VehicleDto>>.Failure("Start date cannot be in the past.");
            }

            var dateRange = new DateRange(search.StartDate, search.EndDate);
            
            // Get available vehicles from repository
            var vehicles = await _unitOfWork.Vehicles.GetAvailableVehiclesAsync(dateRange, search.VehicleTypeId);

            // Apply price filter if specified
            var vehicleList = vehicles.ToList();
            if (search.MaxPricePerDay.HasValue)
            {
                vehicleList = vehicleList
                    .Where(v => GetDailyRate(v) <= search.MaxPricePerDay.Value)
                    .ToList();
            }

            var dtos = vehicleList.Select(MapToDto).ToList();

            return Result<IEnumerable<VehicleDto>>.Success(dtos);
        }

        public async Task<Result<BookingDto>> ReturnVehicleAsync(ReturnVehicleDto dto)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(dto.BookingId);
            if (booking == null)
            {
                return Result<BookingDto>.Failure("Booking not found.");
            }

            if (booking.Status == BookingStatus.Completed)
            {
                return Result<BookingDto>.Failure("Vehicle has already been returned for this booking.");
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                return Result<BookingDto>.Failure("Cannot return vehicle for a cancelled booking.");
            }

            if (booking.Status != BookingStatus.Confirmed)
            {
                return Result<BookingDto>.Failure("Booking must be confirmed before vehicle can be returned.");
            }

            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(booking.VehicleId);
            if (vehicle == null)
            {
                return Result<BookingDto>.Failure("Vehicle not found.");
            }

            // Calculate any late fees
            decimal lateFee = 0;
            if (dto.ActualReturnDate > booking.EndDate)
            {
                var overdueDays = (dto.ActualReturnDate - booking.EndDate).Days;
                if (overdueDays == 0 && dto.ActualReturnDate > booking.EndDate) 
                    overdueDays = 1; // At least 1 day if returned late at all
                
                var dailyRate = GetDailyRate(vehicle);
                lateFee = overdueDays * dailyRate * OverdueMultiplier;
            }

            // Update booking
            booking.Complete();
            if (lateFee > 0)
            {
                booking.TotalAmount = (booking.TotalAmount ?? 0) + lateFee;
                booking.Notes = string.IsNullOrEmpty(booking.Notes) 
                    ? $"Late return fee: {lateFee:C}" 
                    : $"{booking.Notes}\nLate return fee: {lateFee:C}";
            }

            // Add damage notes if applicable
            if (dto.HasDamage && !string.IsNullOrEmpty(dto.DamageDescription))
            {
                booking.Notes = string.IsNullOrEmpty(booking.Notes)
                    ? $"Damage reported: {dto.DamageDescription}"
                    : $"{booking.Notes}\nDamage reported: {dto.DamageDescription}";
                
                // Mark vehicle as needing inspection/maintenance
                vehicle.UpdateStatus(VehicleStatus.InMaintenance);
                vehicle.Issues = dto.DamageDescription;
            }
            else
            {
                // Vehicle is available again
                vehicle.UpdateStatus(VehicleStatus.Available);
            }

            // Add condition notes
            if (!string.IsNullOrEmpty(dto.ConditionNotes))
            {
                booking.Notes = string.IsNullOrEmpty(booking.Notes)
                    ? $"Return condition: {dto.ConditionNotes}"
                    : $"{booking.Notes}\nReturn condition: {dto.ConditionNotes}";
            }

            await _unitOfWork.Bookings.UpdateAsync(booking);
            await _unitOfWork.Vehicles.UpdateAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();

            var client = await _unitOfWork.Clients.GetByIdAsync(booking.ClientId);

            return Result<BookingDto>.Success(MapBookingToDto(booking, vehicle, client));
        }

        private static decimal GetDailyRate(Vehicle vehicle)
        {
            var tariff = vehicle.Tariffs?.FirstOrDefault(t => t.IsActive);
            return tariff?.PricePerDay ?? 50m; // Default daily rate
        }

        private static VehicleDto MapToDto(Vehicle vehicle)
        {
            var dailyRate = GetDailyRate(vehicle);
            
            return new VehicleDto
            {
                Id = vehicle.Id,
                VIN = vehicle.VIN,
                Make = vehicle.Make,
                Model = vehicle.Model,
                Year = vehicle.Year,
                Color = vehicle.Color,
                LicensePlate = vehicle.LicensePlate,
                Status = vehicle.Status.ToString(),
                IsInsured = vehicle.IsInsured,
                DailyRate = dailyRate,
                ImageUrls = vehicle.Images?.Select(i => i.FilePath).ToList() ?? new List<string>()
            };
        }

        private static BookingDto MapBookingToDto(Booking booking, Vehicle? vehicle, Client? client)
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

