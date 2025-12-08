using System;

namespace CarRental.Application.DTOs
{
    public class TariffDto
    {
        public long Id { get; set; }
        public long VehicleId { get; set; }
        public decimal PricePerDay { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
