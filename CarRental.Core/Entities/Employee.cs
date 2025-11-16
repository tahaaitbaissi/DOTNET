using System;

namespace CarRental.Core.Entities
{
    public class Employee : Entity
    {
        public long UserId { get; set; }
        public string Position { get; set; }
        public DateTime HireDate { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }

        public void ProcessReservation()
        {
            throw new NotImplementedException();
        }

        public void HandleSupportTicket()
        {
            throw new NotImplementedException();
        }
    }
}