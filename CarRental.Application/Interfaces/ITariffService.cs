using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface ITariffService
    {
        Task<Result<TariffDto>> AddTariffAsync(CreateTariffDto dto);
        Task<Result<IEnumerable<TariffDto>>> GetVehicleTariffHistoryAsync(long vehicleId);
    }
}
