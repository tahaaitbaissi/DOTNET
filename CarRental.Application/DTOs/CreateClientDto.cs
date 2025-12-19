using System;

namespace CarRental.Application.DTOs
{
    public record CreateClientDto(
        string FullName,
        string Email,
        string Phone,
        string Address,
        string DriverLicense,
        DateTime LicenseExpiry);
}
