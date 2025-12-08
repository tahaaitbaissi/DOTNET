using CarRental.Application.Common.Models;
using CarRental.Application.Interfaces;
using CarRental.Core.Entities;
using CarRental.Core.Enums;
using CarRental.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Infrastructure.Services
{
    public class CsvImportService : IImportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CsvImportService> _logger;

        public CsvImportService(IUnitOfWork unitOfWork, ILogger<CsvImportService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<ImportResultDto>> ImportVehiclesAsync(Stream fileStream)
        {
            try
            {
                using var reader = new StreamReader(fileStream);
                var lines = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        lines.Add(line);
                    }
                }

                if (lines.Count <= 1)
                {
                    return Result<ImportResultDto>.Failure("File is empty or contains only headers.");
                }

                // Assume header: VIN,Make,Model,Year,Color,LicensePlate,VehicleType,PricePerDay,Geometry
                // Simple parsing logic
                var vehiclesToAdd = new List<Vehicle>();
                var vehicleTypes = await _unitOfWork.VehicleTypes.GetAllAsync();
                
                int success = 0;
                int failed = 0;

                // Skip header
                foreach (var line in lines.Skip(1)) 
                {
                    try
                    {
                        var parts = line.Split(',');
                        if (parts.Length < 9)
                        {
                            failed++;
                            continue;
                        }

                        var vin = parts[0].Trim();
                        var existing = await _unitOfWork.Vehicles.GetByVinAsync(vin);
                        if (existing != null)
                        {
                            failed++; // Skip duplicates
                            continue;
                        }

                        var typeName = parts[6].Trim();
                        var type = vehicleTypes.FirstOrDefault(vt => vt.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
                        
                        // Default to first type or specific fallback if not found
                        long? typeId = type?.Id; 
                        
                        var vehicle = new Vehicle
                        {
                            VIN = vin,
                            Make = parts[1].Trim(),
                            Model = parts[2].Trim(),
                            Year = int.Parse(parts[3].Trim()),
                            Color = parts[4].Trim(),
                            LicensePlate = parts[5].Trim(),
                            VehicleTypeId = typeId,
                            Status = VehicleStatus.Available,
                            IsInsured = true,
                            Geometry = int.Parse(parts[8].Trim()),
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        // Create Initial Tariff
                        decimal price = decimal.Parse(parts[7].Trim());
                        vehicle.Tariffs = new List<Tariff>
                        {
                            new Tariff
                            {
                                PricePerDay = price,
                                StartDate = DateTime.UtcNow,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            }
                        };

                        vehiclesToAdd.Add(vehicle);
                        success++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to parse CSV line: {Line}", line);
                        failed++;
                    }
                }

                if (vehiclesToAdd.Any())
                {
                    // Add ranges would be better but repo is generic
                    foreach (var v in vehiclesToAdd)
                    {
                        await _unitOfWork.Vehicles.AddAsync(v);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }

                return Result<ImportResultDto>.Success(new ImportResultDto
                {
                    SuccessCount = success,
                    FailureCount = failed,
                    Message = $"Imported {success} vehicles. Failed: {failed}."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Import failed");
                return Result<ImportResultDto>.Failure($"Import failed: {ex.Message}");
            }
        }
    }
}
