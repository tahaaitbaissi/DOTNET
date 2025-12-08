using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IVehicleTypeService
    {
        Task<Result<IEnumerable<VehicleTypeDto>>> GetAllVehicleTypesAsync();
        Task<Result<VehicleTypeDto>> GetVehicleTypeByIdAsync(long id);
        Task<Result<VehicleTypeDto>> AddVehicleTypeAsync(CreateVehicleTypeDto dto);
        Task<Result<VehicleTypeDto>> UpdateVehicleTypeAsync(long id, UpdateVehicleTypeDto dto);
        Task<Result<bool>> DeleteVehicleTypeAsync(long id);
    }
}
