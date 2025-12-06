using System;

namespace CarRental.Application.DTOs
{
    public record VehicleSearchDto(
        DateTime StartDate,
        DateTime EndDate,
        long? VehicleTypeId = null,
        decimal? MaxPricePerDay = null);
}

