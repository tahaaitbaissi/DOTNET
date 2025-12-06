using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Core.Entities;
using CarRental.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Application.Services
{
    public class VehicleTypeService : IVehicleTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<VehicleTypeDto>>> GetAllVehicleTypesAsync()
        {
            var types = await _unitOfWork.VehicleTypes.GetAllAsync();
            var dtos = types.Select(MapToDto).ToList();
            return Result<IEnumerable<VehicleTypeDto>>.Success(dtos);
        }

        public async Task<Result<VehicleTypeDto>> GetVehicleTypeByIdAsync(long id)
        {
            var type = await _unitOfWork.VehicleTypes.GetByIdAsync(id);
            if (type == null)
            {
                return Result<VehicleTypeDto>.Failure("Vehicle Type not found.");
            }
            return Result<VehicleTypeDto>.Success(MapToDto(type));
        }

        public async Task<Result<VehicleTypeDto>> AddVehicleTypeAsync(CreateVehicleTypeDto dto)
        {
            var type = new VehicleType
            {
                Name = dto.Name,
                Description = dto.Description,
                BaseRate = dto.BaseRate
            };

            await _unitOfWork.VehicleTypes.AddAsync(type);
            await _unitOfWork.SaveChangesAsync();

            return Result<VehicleTypeDto>.Success(MapToDto(type));
        }

        public async Task<Result<VehicleTypeDto>> UpdateVehicleTypeAsync(long id, UpdateVehicleTypeDto dto)
        {
            var type = await _unitOfWork.VehicleTypes.GetByIdAsync(id);
            if (type == null)
            {
                return Result<VehicleTypeDto>.Failure("Vehicle Type not found.");
            }

            type.Name = dto.Name;
            type.Description = dto.Description;
            type.BaseRate = dto.BaseRate;

            await _unitOfWork.VehicleTypes.UpdateAsync(type);
            await _unitOfWork.SaveChangesAsync();

            return Result<VehicleTypeDto>.Success(MapToDto(type));
        }

        public async Task<Result<bool>> DeleteVehicleTypeAsync(long id)
        {
            var type = await _unitOfWork.VehicleTypes.GetByIdAsync(id);
            if (type == null)
            {
                return Result<bool>.Failure("Vehicle Type not found.");
            }

            // Check if any vehicles use this type?
            // Since we don't have a direct query for this here easily unless we use explicit filtering
            // and repository.DeleteAsync usually just deletes
            // For now, assume cascading restrict as defined in configuration will throw exception if in use.
            
            try 
            {
                await _unitOfWork.VehicleTypes.DeleteAsync(type);
                await _unitOfWork.SaveChangesAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure("Cannot delete this type because it is in use by one or more vehicles.");
            }
        }

        private static VehicleTypeDto MapToDto(VehicleType type)
        {
            return new VehicleTypeDto
            {
                Id = type.Id,
                Name = type.Name,
                Description = type.Description,
                BaseRate = type.BaseRate
            };
        }
    }
}
