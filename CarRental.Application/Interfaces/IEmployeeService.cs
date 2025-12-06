using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<Result<IEnumerable<EmployeeDto>>> GetAllEmployeesAsync();
        Task<Result<EmployeeDto>> GetEmployeeByIdAsync(long id);
        Task<Result<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto dto);
        Task<Result<bool>> DeleteEmployeeAsync(long id);
        // Note: Update logic can be added similarly
    }
}
