using CarRental.Application.Interfaces;
using CarRental.Core.Interfaces;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace CarRental.Infrastructure.Services
{
    public class CsvExportService : IExportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CsvExportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<byte[]> ExportVehiclesAsync()
        {
            var vehicles = await _unitOfWork.Vehicles.GetAllAsync();
            var args = new StringBuilder();
            
            // Header
            args.AppendLine("Id,Make,Model,Year,LicensePlate,Status,Mileage,BaseRate");

            foreach (var v in vehicles)
            {
                // Escape logic could be added here for robust CSV
                var line = $"{v.Id},{Escape(v.Make)},{Escape(v.Model)},{v.Year},{Escape(v.LicensePlate)},{v.Status},{v.CurrentMileage},{v.VehicleTypeId}";
                args.AppendLine(line);
            }

            return Encoding.UTF8.GetBytes(args.ToString());
        }

        public async Task<byte[]> ExportBookingsAsync()
        {
            var bookings = await _unitOfWork.Bookings.GetAllAsync();
            var args = new StringBuilder();

            args.AppendLine("Id,VehicleId,ClientId,StartDate,EndDate,TotalAmount,Status,IsPaid");

            foreach (var b in bookings)
            {
                var line = $"{b.Id},{b.VehicleId},{b.ClientId},{b.StartDate:yyyy-MM-dd},{b.EndDate:yyyy-MM-dd},{b.TotalAmount},{b.Status},{b.IsPaid}";
                args.AppendLine(line);
            }

            return Encoding.UTF8.GetBytes(args.ToString());
        }

        public async Task<byte[]> ExportClientsAsync()
        {
            var clients = await _unitOfWork.Clients.GetAllAsync();
            // Need user data? 
            // Client entity logic might need Include. UnitOfWork.Clients.GetAllAsync might not return User.
            // For export, we usually want names.
            // I'll grab generic list.
            
            var args = new StringBuilder();
            args.AppendLine("Id,UserId,DriverLicense,Phone,Address");

            foreach (var c in clients)
            {
                var line = $"{c.Id},{c.UserId},{Escape(c.DriverLicense)},{Escape(c.Phone)},{Escape(c.Address)}";
                args.AppendLine(line);
            }

            return Encoding.UTF8.GetBytes(args.ToString());
        }

        private string Escape(string val)
        {
            if (string.IsNullOrEmpty(val)) return "";
            if (val.Contains(",") || val.Contains("\"") || val.Contains("\n"))
            {
                return $"\"{val.Replace("\"", "\"\"")}\"";
            }
            return val;
        }
    }
}
