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
            // GET /api/VehicleTypes
            var result = await _apiClient.GetAsync<List<VehicleTypeDto>>("api/VehicleTypes");
            return result ?? new List<VehicleTypeDto>();
        }

        public async Task AddVehicleTypeAsync(CreateVehicleTypeDto vehicleType)
        {
             // POST /api/VehicleTypes
             await _apiClient.PostAsync("api/VehicleTypes", vehicleType);
        }

        public async Task UpdateVehicleTypeAsync(VehicleTypeDto vehicleType)
        {
             // PUT /api/VehicleTypes/{id}
             await _apiClient.PutAsync($"api/VehicleTypes/{vehicleType.Id}", vehicleType);
        }

        public async Task DeleteVehicleTypeAsync(long id)
        {
             // DELETE /api/VehicleTypes/{id}
             await _apiClient.DeleteAsync($"api/VehicleTypes/{id}");
        }
    }
}
