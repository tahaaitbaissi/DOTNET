using System.Collections.Generic;

namespace CarRental.Core.Entities
{
    public class VehicleType : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BaseRate { get; set; }

        // Navigation Properties
        public virtual ICollection<Vehicle> Vehicles { get; set; }
        public virtual ICollection<Tariff> Tariffs { get; set; }

        // Methods from diagram
        public List<Vehicle> GetVehicles()
        {
            throw new NotImplementedException();
        }

        public List<Tariff> GetActiveTariffs()
        {
            throw new NotImplementedException();
        }
    }
}