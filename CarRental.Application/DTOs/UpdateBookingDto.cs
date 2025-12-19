using System;

namespace CarRental.Application.DTOs
{
    public record UpdateBookingDto(
        long VehicleId,
        DateTime StartDate,
        DateTime EndDate,
        string PickUpLocation,
        string DropOffLocation,
        string? Notes = null);
}
