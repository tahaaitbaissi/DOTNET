using System.Collections.Generic;
using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Desktop.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeDto>> GetAllEmployeesAsync();
        Task AddEmployeeAsync(CarRental.Application.DTOs.CreateEmployeeDto employee);
        Task UpdateEmployeeAsync(long id, CarRental.Application.DTOs.EmployeeDto employee);
        Task DeleteEmployeeAsync(long id);
    }

    public class ApiEmployeeService : IEmployeeService
    {
        private readonly IApiClient _apiClient;

        public ApiEmployeeService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
            // GET /api/Employees
            var result = await _apiClient.GetAsync<List<EmployeeDto>>("api/Employees");
            return result ?? new List<EmployeeDto>();
        }

        public async Task AddEmployeeAsync(CreateEmployeeDto employee)
        {
            // POST /api/Employees
            await _apiClient.PostAsync("api/Employees", employee);
        }

        public async Task UpdateEmployeeAsync(long id, EmployeeDto employee)
        {
            // PUT /api/Employees/{id}
            // Note: Use UpdateEmployeeDto if exists, but Interface here uses EmployeeDto mostly
            // Checking DTOs list, I saw CreateEmployeeDto, maybe UpdateEmployeeDto exists?
            // Will check compilation. For now use EmployeeDto or appropriate one.
            await _apiClient.PutAsync($"api/Employees/{id}", employee);
        }

        public async Task DeleteEmployeeAsync(long id)
        {
            // DELETE /api/Employees/{id}
            await _apiClient.DeleteAsync($"api/Employees/{id}");
        }
    }
}
