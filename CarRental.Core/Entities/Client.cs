using System;
using System.Collections.Generic;

namespace CarRental.Core.Entities
{
    public class Client : AuditableEntity
    {
        public long UserId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string DriverLicense { get; set; }
        public DateTime LicenseExpiry { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Document> Documents { get; set; }

        public List<Booking> GetActiveBookings()
        {
            throw new NotImplementedException();
        }

        public List<Payment> GetPaymentMethods()
        {
            throw new NotImplementedException();
        }
    }
}