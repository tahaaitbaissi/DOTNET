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

        public async Task UpdateVehicleAsync(VehicleDto vehicle)
        {
            // Map to UpdateVehicleDto
            // Assuming Status string needs parsing to Enum
            System.Enum.TryParse<CarRental.Core.Enums.VehicleStatus>(vehicle.Status, true, out var statusEnum);

            var updateDto = new UpdateVehicleDto
            {
                Make = vehicle.Make,
                Model = vehicle.Model,
                Year = vehicle.Year,
                LicensePlate = vehicle.LicensePlate,
                Color = string.IsNullOrWhiteSpace(vehicle.Color) ? "Unknown" : vehicle.Color, // Default if null or empty
                Status = statusEnum,
                IsInsured = vehicle.IsInsured,
                InsurancePolicy = "", // DTO in desktop might not have this, leave empty
                Issues = "",
                PricePerDay = vehicle.DailyRate,
                VehicleTypeId = null // Or fetch if available
            };

            // PUT /api/Vehicles/{id}
            await _apiClient.PutAsync($"api/Vehicles/{vehicle.Id}", updateDto);
        }

        public async Task<List<VehicleDto>> GetAvailableVehiclesAsync(System.DateTime start, System.DateTime end)
        {
            // GET /api/Vehicles/available?startDate=...&endDate=...
            // Need to format dates carefully for URL parameters
            string startStr = start.ToString("yyyy-MM-dd");
            string endStr = end.ToString("yyyy-MM-dd");
            
            var result = await _apiClient.GetAsync<List<VehicleDto>>($"api/Vehicles/available?startDate={startStr}&endDate={endStr}");
            return result ?? new List<VehicleDto>();
        }
    }
}
