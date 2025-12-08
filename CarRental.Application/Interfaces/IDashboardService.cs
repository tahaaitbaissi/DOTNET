using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<Result<DashboardDto>> GetDashboardDataAsync();
    }
}
