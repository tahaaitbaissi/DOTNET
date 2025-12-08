using System;

namespace CarRental.Application.DTOs
{
    public record ReturnVehicleDto(
        long BookingId,
        DateTime ActualReturnDate,
        decimal? FinalMileage = null,
        string? ConditionNotes = null,
        bool HasDamage = false,
        string? DamageDescription = null);
}

