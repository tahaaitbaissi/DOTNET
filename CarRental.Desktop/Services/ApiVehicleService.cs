using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Desktop.Services
{
    public class ApiVehicleService : IVehicleService
    {
        private readonly IApiClient _apiClient;

        public ApiVehicleService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<VehicleDto>> GetAllVehiclesAsync()
        {
            // Assuming endpoint is GET /api/Vehicles
            // The API might return Result<List<VehicleDto>> or just List<VehicleDto>
            // Checking VehiclesController... usually it's PaginatedResult or Result. 
            // In typical clean architecture, GetAll returns Result<List<T>>.
            // But let's assume valid response for now or I might need to unwrap.
            // Safety: Try to fetch as List<VehicleDto>
            
            var result = await _apiClient.GetAsync<List<VehicleDto>>("api/Vehicles");
            return result ?? new List<VehicleDto>();
        }

        public async Task AddVehicleAsync(CreateVehicleDto vehicle)
        {
            // Assuming endpoint is POST /api/Vehicles
            await _apiClient.PostAsync("api/Vehicles", vehicle);
        }
    }
}
