using System;

namespace CarRental.Application.DTOs
{
    public class CreateTariffDto
    {
        public long VehicleId { get; set; }
        public decimal PricePerDay { get; set; }
        public DateTime StartDate { get; set; }
    }
}
