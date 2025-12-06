using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IExportService
    {
        Task<byte[]> ExportVehiclesAsync();
        Task<byte[]> ExportBookingsAsync();
        Task<byte[]> ExportClientsAsync();
    }
}
