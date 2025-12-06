using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Core.Entities;
using CarRental.Core.Interfaces;
using CarRental.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace CarRental.Application.Services
{
    /// <summary>
    /// Authentication service implementation
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IEmailService _emailService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IDateTimeProvider dateTimeProvider,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _dateTimeProvider = dateTimeProvider;
            _emailService = emailService;
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            // Find user by email
            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                return Result<AuthResponseDto>.Failure("Invalid email or password.");
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return Result<AuthResponseDto>.Failure("This account has been deactivated.");
            }

            // Verify password
            if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return Result<AuthResponseDto>.Failure("Invalid email or password.");
            }

            // Generate token
            var token = _tokenService.GenerateToken(user);

            // Create response
            var response = new AuthResponseDto
            {
                Token = token,
                ExpiresAt = _dateTimeProvider.UtcNow.AddHours(1), // Match JWT settings
                User = MapToUserInfo(user)
            };

            return Result<AuthResponseDto>.Success(response);
        }

        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            // Check if email already exists
            if (await _unitOfWork.Users.ExistsWithEmailAsync(dto.Email))
            {
                return Result<AuthResponseDto>.Failure("An account with this email already exists.");
            }

            // Check if username already exists
            if (await _unitOfWork.Users.ExistsWithUsernameAsync(dto.Username))
            {
                return Result<AuthResponseDto>.Failure("This username is already taken.");
            }

            // Create user
            var verificationCode = new Random().Next(100000, 999999).ToString();

            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                FullName = dto.FullName,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                IsActive = true, // We allow login but maybe restrict features until verified? 
                                 // Or set false to enforce verification before login.
                                 // For now, let's keep true but track verification.
                IsEmailVerified = false,
                VerificationToken = verificationCode,
                VerificationTokenExpiry = _dateTimeProvider.UtcNow.AddMinutes(15),
                CreatedAt = _dateTimeProvider.UtcNow,
                UpdatedAt = _dateTimeProvider.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Send Verification Email
            try
            {
                await _emailService.SendAccountVerificationEmailAsync(user.Email, verificationCode);
            }
            catch
            {
                // Log warning: Email failed, user might need to resend
            }

            // Create client profile
            var client = new Client
            {
                UserId = user.Id,
                Phone = dto.Phone,
                Address = dto.Address ?? string.Empty,
                DriverLicense = dto.DriverLicense ?? string.Empty,
                LicenseExpiry = dto.LicenseExpiry ?? _dateTimeProvider.UtcNow.AddYears(5),
                CreatedAt = _dateTimeProvider.UtcNow,
                UpdatedAt = _dateTimeProvider.UtcNow
            };

            await _unitOfWork.Clients.AddAsync(client);
            await _unitOfWork.SaveChangesAsync();

            // Reload user with client info
            user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);

            // Generate token
            var token = _tokenService.GenerateToken(user!);

            // Create response
            var response = new AuthResponseDto
            {
                Token = token,
                ExpiresAt = _dateTimeProvider.UtcNow.AddHours(1),
                User = MapToUserInfo(user!)
            };

            return Result<AuthResponseDto>.Success(response);
        }

        public Task<Result<bool>> ValidateTokenAsync(string token)
        {
            var isValid = _tokenService.ValidateToken(token);
            return Task.FromResult(Result<bool>.Success(isValid));
        }

        public async Task<Result<bool>> VerifyEmailAsync(VerifyEmailDto dto)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                return Result<bool>.Failure("User not found.");
            }

            if (user.IsEmailVerified)
            {
                return Result<bool>.Failure("Email is already verified.");
            }

            if (user.VerificationToken != dto.Token)
            {
                return Result<bool>.Failure("Invalid verification code.");
            }

            if (user.VerificationTokenExpiry < _dateTimeProvider.UtcNow)
            {
                return Result<bool>.Failure("Verification code has expired.");
            }

            // Verify
            user.IsEmailVerified = true;
            user.VerificationToken = null;
            user.VerificationTokenExpiry = null;
            user.UpdatedAt = _dateTimeProvider.UtcNow;

            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ForgotPasswordAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal user existence
                return Result<bool>.Success(true);
            }

            // Generate Token (UUID)
            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = _dateTimeProvider.UtcNow.AddHours(1);
            user.UpdatedAt = _dateTimeProvider.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            // Send Email
            try
            {
                await _emailService.SendPasswordResetEmailAsync(user.Email, token);
            }
            catch
            {
                // Log failure
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (user == null)
            { 
                 return Result<bool>.Failure("Invalid request.");
            }

            if (user.PasswordResetToken != dto.Token)
            {
                 return Result<bool>.Failure("Invalid or invalid token.");
            }

            if (user.PasswordResetTokenExpiry < _dateTimeProvider.UtcNow)
            {
                 return Result<bool>.Failure("Token has expired.");
            }

            // Update Password
            user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            user.UpdatedAt = _dateTimeProvider.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        private static UserInfoDto MapToUserInfo(User user)
        {
            var role = "User";
            if (user.Employee != null)
            {
                role = "Employee";
            }
            else if (user.Client != null)
            {
                role = "Client";
            }

            return new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                FullName = user.FullName,
                Role = role,
                ClientId = user.Client?.Id,
                EmployeeId = user.Employee?.Id
            };
        }
    }
}

