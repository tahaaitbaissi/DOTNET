using System.Threading.Tasks;

namespace CarRental.Desktop.Services
{
    public interface IReportGeneratorService
    {
        // Rapports simples
        Task GenerateDailyReportAsync();
        Task GenerateMonthlyReportAsync();

        // Export simple
        Task<bool> ExportBookingsReportAsync();
        Task<bool> ExportClientsReportAsync();
    }
}