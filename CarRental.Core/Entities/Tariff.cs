using System;

namespace CarRental.Core.Entities
{
    public class Tariff : AuditableEntity
    {
        public long? VehicleTypeId { get; set; }
        public long? VehicleId { get; set; }
        public decimal? PricePerHour { get; set; }
        public decimal? PricePerDay { get; set; }
        public decimal? PricePerKm { get; set; }
        public string Currency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }

        // Navigation Properties
        public virtual VehicleType VehicleType { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        public decimal CalculateCost(DateTime startTime, DateTime endTime, decimal distance)
        {
            throw new NotImplementedException();
        }

        public bool IsValidForDate(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}