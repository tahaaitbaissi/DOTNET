using System.Collections.Generic;

namespace CarRental.Application.DTOs
{
    public class DashboardDto
    {
        public int TotalVehicles { get; set; }
        public int AvailableVehicles { get; set; }
        public int RentedVehicles { get; set; }
        public int InMaintenanceVehicles { get; set; }
        
        public int TotalClients { get; set; }
        public int ActiveBookings { get; set; }
        
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        
        public List<BookingDto> RecentBookings { get; set; }
        public List<string> Alerts { get; set; } = new List<string>();
    }
}
