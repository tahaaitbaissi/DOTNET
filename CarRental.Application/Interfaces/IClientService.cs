using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IClientService
    {
        Task<Result<IEnumerable<ClientDto>>> GetAllClientsAsync();
        Task<Result<ClientDto>> GetClientByIdAsync(long id);
        Task<Result<ClientDto>> UpdateClientAsync(long id, UpdateClientDto dto);
        Task<Result<bool>> DeleteClientAsync(long id);
    }
}
