using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Desktop.Models;

namespace CarRental.Desktop.Services
{
    public interface IClientService
    {
        Task<List<Client>> GetAllClientsAsync();
        Task AddClientAsync(Client client);
        Task DeleteClientAsync(int id);
    }

    public class MockClientService : IClientService
    {
        private readonly List<Client> _clients;

        public MockClientService()
        {
            _clients = new List<Client>
            {
                new Client { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Phone = "555-0101", LicenseNumber = "DL123456" },
                new Client { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Phone = "555-0102", LicenseNumber = "DL654321" },
                new Client { Id = 3, FirstName = "Alice", LastName = "Johnson", Email = "alice@example.com", Phone = "555-0103", LicenseNumber = "DL789012" },
                new Client { Id = 4, FirstName = "Bob", LastName = "Williams", Email = "bob.w@example.com", Phone = "555-0104", LicenseNumber = "DL999888" },
                new Client { Id = 5, FirstName = "Charlie", LastName = "Brown", Email = "charlie@example.com", Phone = "555-0105", LicenseNumber = "DL777666" },
                new Client { Id = 6, FirstName = "Diana", LastName = "Prince", Email = "diana@example.com", Phone = "555-0106", LicenseNumber = "DL333222" },
                new Client { Id = 7, FirstName = "Evan", LastName = "Wright", Email = "evan@example.com", Phone = "555-0107", LicenseNumber = "DL111000" }
            };
        }

        public Task<List<Client>> GetAllClientsAsync()
        {
            return Task.FromResult(_clients.ToList());
        }

        public Task AddClientAsync(Client client)
        {
            client.Id = _clients.Max(c => c.Id) + 1;
            _clients.Add(client);
            return Task.CompletedTask;
        }

        public Task DeleteClientAsync(int id)
        {
            var client = _clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
            {
                _clients.Remove(client);
            }
            return Task.CompletedTask;
        }
    }
}
