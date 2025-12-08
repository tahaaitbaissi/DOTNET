using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IMaintenanceService
    {
        Task<Result<Maintenance>> ScheduleMaintenanceAsync(CreateMaintenanceDto dto);
        Task<Result<bool>> CompleteMaintenanceAsync(CompleteMaintenanceDto dto);
        Task<Result<IEnumerable<Maintenance>>> GetActiveMaintenancesAsync();
        
        // Triggers alerts check
        Task<Result<int>> CheckAndGenerateAlertsAsync(); 
        
        Task<Result<IEnumerable<Notification>>> GetAdminAlertsAsync();
    }
}
