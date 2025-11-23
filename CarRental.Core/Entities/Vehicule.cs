using System;
using System.Collections.Generic;
using CarRental.Core.Enums;

namespace CarRental.Core.Entities
{
    public class Vehicle : AuditableEntity
    {
        public string VIN { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public string LicensePlate { get; set; }
        public VehicleStatus Status { get; set; }
        public bool IsInsured { get; set; }
        public string InsurancePolicy { get; set; }
        public int Geometry { get; set; }
        public string Issues { get; set; }

        // Navigation Properties
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<VehicleImage> Images { get; set; }
        public virtual ICollection<Maintenance> MaintenanceHistory { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<Tariff> Tariffs { get; set; }

        // Methods from diagram
        public bool CheckAvailability(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(VehicleStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public List<Maintenance> GetMaintenanceHistory()
        {
            throw new NotImplementedException();
        }
    }
}