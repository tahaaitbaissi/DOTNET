using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Desktop.Services
{
    public interface IVehicleTypeService
    {
        Task<List<VehicleTypeDto>> GetAllVehicleTypesAsync();
        // Assuming standard CRUD exists or just Read for dropdowns
        Task AddVehicleTypeAsync(CarRental.Application.DTOs.CreateVehicleTypeDto vehicleType);
        Task UpdateVehicleTypeAsync(CarRental.Application.DTOs.VehicleTypeDto vehicleType);
        Task DeleteVehicleTypeAsync(long id);
    }

    public class ApiVehicleTypeService : IVehicleTypeService
    {
        private readonly IApiClient _apiClient;

        public ApiVehicleTypeService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<VehicleTypeDto>> GetAllVehicleTypesAsync()
        {
            // GET /api/vehicle-types
            var result = await _apiClient.GetAsync<List<VehicleTypeDto>>("api/vehicle-types");
            return result ?? new List<VehicleTypeDto>();
        }

        public async Task AddVehicleTypeAsync(CreateVehicleTypeDto vehicleType)
        {
             // POST /api/vehicle-types
             await _apiClient.PostAsync("api/vehicle-types", vehicleType);
        }

        public async Task UpdateVehicleTypeAsync(VehicleTypeDto vehicleType)
        {
             // Map to UpdateDto
             var updateDto = new CarRental.Application.DTOs.UpdateVehicleTypeDto
             {
                 Name = vehicleType.Name,
                 Description = vehicleType.Description,
                 BaseRate = vehicleType.BaseRate
             };

             // PUT /api/vehicle-types/{id}
             await _apiClient.PutAsync($"api/vehicle-types/{vehicleType.Id}", updateDto);
        }

        public async Task DeleteVehicleTypeAsync(long id)
        {
             // DELETE /api/vehicle-types/{id}
             await _apiClient.DeleteAsync($"api/vehicle-types/{id}");
        }
    }
}
