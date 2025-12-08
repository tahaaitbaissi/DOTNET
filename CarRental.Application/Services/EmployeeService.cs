using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Core.Entities;
using CarRental.Core.Interfaces;
using CarRental.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public EmployeeService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<IEnumerable<EmployeeDto>>> GetAllEmployeesAsync()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            
            // Need to load User data for mapping (Repo GetAll might not include navigation properties by default)
            // But EmployeeRepository implementation relies on generic repository, which usually doesn't include without spec.
            // However, GetByEmail/Id in Repo does. 
            // For now, let's assume lazy loading or we iterate. 
            // Better practice: Use a specific query in repo. But for now speed: 
            var result = new List<EmployeeDto>();
            foreach(var emp in employees)
            {
                // Re-fetch with User if needed, or rely on lazy loading if enabled. 
                // Best safe bet:
                var fullEmp = await _unitOfWork.Employees.GetByUserIdAsync(emp.UserId);
                if(fullEmp != null) result.Add(MapToDto(fullEmp));
            }

            return Result<IEnumerable<EmployeeDto>>.Success(result);
        }

        public async Task<Result<EmployeeDto>> GetEmployeeByIdAsync(long id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null) return Result<EmployeeDto>.Failure("Employee not found.");
            
            var fullEmp = await _unitOfWork.Employees.GetByUserIdAsync(employee.UserId);
            return Result<EmployeeDto>.Success(MapToDto(fullEmp));
        }

        public async Task<Result<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            // Transactional logic is implicit via UnitOfWork SaveChanges
            
            // 1. Check if user exists
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return Result<EmployeeDto>.Failure("User with this email already exists.");
            }

            // 2. Create User
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Username = dto.Username,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(); // Save to get User Id

            // 3. Create Employee
            var employee = new Employee
            {
                UserId = user.Id,
                Position = dto.Position,
                HireDate = dto.HireDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            // Return DTO
            user.Employee = employee; // Link for mapping
            employee.User = user;
            
            return Result<EmployeeDto>.Success(MapToDto(employee));
        }

        public async Task<Result<bool>> DeleteEmployeeAsync(long id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null) return Result<bool>.Failure("Employee not found.");

            // Also delete (or deactivate) User?
            var user = await _unitOfWork.Users.GetByIdAsync(employee.UserId);
            if (user != null)
            {
                user.IsActive = false; // Soft delete user
                await _unitOfWork.Users.UpdateAsync(user);
            }

            await _unitOfWork.Employees.DeleteAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        private static EmployeeDto MapToDto(Employee emp)
        {
            return new EmployeeDto
            {
                Id = emp.Id,
                UserId = emp.UserId,
                FullName = emp.User?.FullName ?? "Unknown",
                Email = emp.User?.Email ?? "Unknown",
                Position = emp.Position,
                HireDate = emp.HireDate,
                IsActive = emp.User?.IsActive ?? false
            };
        }
    }
}
