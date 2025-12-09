using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Desktop.Services
{
    public interface IClientService
    {
        Task<List<ClientDto>> GetAllClientsAsync();
        // Client creation might be complex (Authentication/Registration), 
        // usually handled by AuthController register?
        // Or if Admin creates client, using ClientsController.Create?
        // Checking backend: ClientsController usually has Get, Update, etc.
        // Let's assume standard CRUD for now.
        // Task AddClientAsync(CreateClientDto client); 
        Task UpdateClientAsync(long id, UpdateClientDto client);
        Task DeleteClientAsync(long id);
    }

    public class ApiClientService : IClientService
    {
        private readonly IApiClient _apiClient;

        public ApiClientService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<ClientDto>> GetAllClientsAsync()
        {
            // GET /api/Clients
            var result = await _apiClient.GetAsync<List<ClientDto>>("api/Clients");
            return result ?? new List<ClientDto>();
        }

        public async Task UpdateClientAsync(long id, UpdateClientDto client)
        {
            // PUT /api/Clients/{id}
            await _apiClient.PutAsync($"api/Clients/{id}", client);
        }

        public async Task DeleteClientAsync(long id)
        {
            // DELETE /api/Clients/{id}
            await _apiClient.DeleteAsync($"api/Clients/{id}");
        }
    }
}
