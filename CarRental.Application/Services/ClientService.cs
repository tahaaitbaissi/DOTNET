using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Core.Entities;
using CarRental.Core.Interfaces;
using CarRental.Core.Interfaces.Services;
using CarRental.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public ClientService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<IEnumerable<ClientDto>>> GetAllClientsAsync()
        {
            var clients = await _unitOfWork.Clients.GetAllAsync();
            var dtos = new List<ClientDto>();

            foreach (var client in clients)
            {
                // We need to load User info. 
                // In a optimized scenario, we would use .Include(c => c.User) in the repository.
                // Assuming EF Core lazy loading or repository handling.
                var user = await _unitOfWork.Users.GetByIdAsync(client.UserId);
                dtos.Add(MapToDto(client, user));
            }

            return Result<IEnumerable<ClientDto>>.Success(dtos);
        }

        public async Task<Result<ClientDto>> GetClientByIdAsync(long id)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(id);
            if (client == null)
            {
                return Result<ClientDto>.Failure("Client not found.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(client.UserId);
            return Result<ClientDto>.Success(MapToDto(client, user));
        }

        public async Task<Result<ClientDto>> GetClientByUserIdAsync(long userId)
        {
            var clients = await _unitOfWork.Clients.GetAllAsync();
            var client = clients.FirstOrDefault(c => c.UserId == userId);
            
            if (client == null)
            {
                return Result<ClientDto>.Failure("Client not found for this user.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return Result<ClientDto>.Success(MapToDto(client, user));
        }

        public async Task<Result<ClientDto>> CreateClientAsync(CreateClientDto dto)
        {
            // Check if user with email already exists
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return Result<ClientDto>.Failure("User with this email already exists.");
            }

            // Create User account
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Username = dto.Email, // Use email as username
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailVerified = true, // Auto-verify for admin created clients
                RoleId = (long)UserRole.Client,
                PasswordHash = _passwordHasher.HashPassword("Client@123") // Default password
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(); // Save user to get Id

            // Create Client profile
            var client = new Client
            {
                UserId = user.Id,
                Phone = dto.Phone,
                Address = dto.Address,
                DriverLicense = dto.DriverLicense,
                LicenseExpiry = dto.LicenseExpiry,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Clients.AddAsync(client);
            await _unitOfWork.SaveChangesAsync();

            return Result<ClientDto>.Success(MapToDto(client, user));
        }

        public async Task<Result<ClientDto>> UpdateClientAsync(long id, UpdateClientDto dto)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(id);
            if (client == null)
            {
                return Result<ClientDto>.Failure("Client not found.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(client.UserId);
            if (user == null)
            {
                return Result<ClientDto>.Failure("Associated user account not found.");
            }

            // Update Client Info
            client.Phone = dto.Phone;
            client.Address = dto.Address ?? string.Empty;
            client.DriverLicense = dto.DriverLicense;
            client.LicenseExpiry = dto.LicenseExpiry;
            client.UpdatedAt = DateTime.UtcNow;

            // Update User Info
            user.FullName = dto.FullName;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Clients.UpdateAsync(client);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<ClientDto>.Success(MapToDto(client, user));
        }

        public async Task<Result<bool>> DeleteClientAsync(long id)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(id);
            if (client == null)
            {
                return Result<bool>.Failure("Client not found.");
            }

            // Hard delete usually not recommended for Users/Clients, but fulfilling CRUD requirement.
            // Check for bookings first?
            var bookings = await _unitOfWork.Bookings.GetBookingsForClientAsync(id);
            if (bookings.Any())
            {
                return Result<bool>.Failure("Cannot delete client with existing bookings.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(client.UserId);

            await _unitOfWork.Clients.DeleteAsync(client);
            if (user != null)
            {
                await _unitOfWork.Users.DeleteAsync(user);
            }
            
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        private static ClientDto MapToDto(Client client, User? user)
        {
            return new ClientDto
            {
                Id = client.Id,
                UserId = client.UserId,
                Email = user?.Email ?? string.Empty,
                Username = user?.Username ?? string.Empty,
                FullName = user?.FullName ?? string.Empty,
                Phone = client.Phone,
                Address = client.Address,
                DriverLicense = client.DriverLicense,
                LicenseExpiry = client.LicenseExpiry,
                IsActive = user?.IsActive ?? false,
                IsEmailVerified = user?.IsEmailVerified ?? false,
                CreatedAt = client.CreatedAt
            };
        }
    }
}
