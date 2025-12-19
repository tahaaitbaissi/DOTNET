using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Desktop.Services
{
    public interface IDashboardService
    {
        Task<CarRental.Application.Common.Models.Result<DashboardDto>> GetDashboardDataAsync();
    }

    public class ApiDashboardService : IDashboardService
    {
        private readonly IApiClient _apiClient;

        public ApiDashboardService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<CarRental.Application.Common.Models.Result<DashboardDto>> GetDashboardDataAsync()
        {
            // GET /api/Dashboard
            var result = await _apiClient.GetAsync<DashboardDto>("api/Dashboard");
            if (result != null)
            {
                return CarRental.Application.Common.Models.Result<DashboardDto>.Success(result);
            }
            return CarRental.Application.Common.Models.Result<DashboardDto>.Failure("Failed to load dashboard data.");
        }
    }
}
