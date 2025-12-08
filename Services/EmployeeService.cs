using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Desktop.Models;

namespace CarRental.Desktop.Services
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllEmployeesAsync();
        Task AddEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
    }

    public class MockEmployeeService : IEmployeeService
    {
        private readonly List<Employee> _employees;

        public MockEmployeeService()
        {
            _employees = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "Admin", LastName = "User", Role = "Manager", Email = "admin@carrental.com", Phone = "555-1000" },
                new Employee { Id = 2, FirstName = "Bob", LastName = "Builder", Role = "Sales", Email = "bob@carrental.com", Phone = "555-1001" },
                new Employee { Id = 3, FirstName = "Charlie", LastName = "Mechanic", Role = "Maintenance", Email = "charlie@carrental.com", Phone = "555-1002" }
            };
        }

        public Task<List<Employee>> GetAllEmployeesAsync()
        {
            return Task.FromResult(_employees.ToList());
        }

        public Task AddEmployeeAsync(Employee employee)
        {
            employee.Id = _employees.Max(e => e.Id) + 1;
            _employees.Add(employee);
            return Task.CompletedTask;
        }

        public Task DeleteEmployeeAsync(int id)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            if (employee != null)
            {
                _employees.Remove(employee);
            }
            return Task.CompletedTask;
        }
    }
}
