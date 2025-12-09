using System;
using System.Threading.Tasks;
using CarRental.Application.DTOs;
using CarRental.Application.Common.Models; // for Result<T>

namespace CarRental.Desktop.Services
{
    public interface IDashboardService
    {
        Task<Result<DashboardDto>> GetDashboardDataAsync();
    }

    public class ApiDashboardService : IDashboardService
    {
        private readonly IApiClient _apiClient;

        public ApiDashboardService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Result<DashboardDto>> GetDashboardDataAsync()
        {
            // The backend endpoint returns ActionResult<DashboardDto>, 
            // which usually serializes to just DashboardDto on success.
            // But checking DashboardController:
            // return Ok(result.Value); where result is Result<DashboardDto>
            // Wait, DashboardController:
            // var result = await _dashboardService.GetDashboardDataAsync();
            // return Ok(result.Value);
            // This means the API returns the DashboardDto JSON directly.
            
            var data = await _apiClient.GetAsync<DashboardDto>("api/Dashboard");
            if (data != null)
            {
                return Result<DashboardDto>.Success(data);
            }
            return Result<DashboardDto>.Failure("Failed to load dashboard data.");
        }
    }
}
