using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IVehicleService
    {
        /// <summary>
        /// Gets vehicle details by ID
        /// </summary>
        Task<Result<VehicleDto>> GetVehicleByIdAsync(long vehicleId);

        /// <summary>
        /// Gets all vehicles (Admin)
        /// </summary>
        Task<Result<IEnumerable<VehicleDto>>> GetAllVehiclesAsync();
        
        /// <summary>
        /// Create a new vehicle
        /// </summary>
        Task<Result<VehicleDto>> AddVehicleAsync(CreateVehicleDto dto);

        /// <summary>
        /// Update an existing vehicle
        /// </summary>
        Task<Result<VehicleDto>> UpdateVehicleAsync(long id, UpdateVehicleDto dto);

        /// <summary>
        /// Delete a vehicle
        /// </summary>
        Task<Result<bool>> DeleteVehicleAsync(long id);

        /// <summary>
        /// Upload a vehicle image
        /// </summary>
        Task<Result<string>> AddVehicleImageAsync(long vehicleId, Stream fileStream, string fileName);

        /// <summary>
        /// Remove a vehicle image
        /// </summary>
        Task<Result<bool>> RemoveVehicleImageAsync(long imageId);
        
        /// <summary>
        /// Searches for available vehicles within a date range and optional filters
        /// </summary>
        Task<Result<IEnumerable<VehicleDto>>> GetAvailableVehiclesAsync(VehicleSearchDto search);
        
        /// <summary>
        /// Processes vehicle return, calculates any late fees, and updates booking status
        /// </summary>
        Task<Result<BookingDto>> ReturnVehicleAsync(ReturnVehicleDto dto);
    }
}

