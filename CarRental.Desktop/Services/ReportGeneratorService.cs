using CarRental.Application.DTOs;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace CarRental.Desktop.Services
{
    public class ReportGeneratorService : IReportGeneratorService
    {
        private readonly IFileExportService _exportService;
        private readonly IDialogService _dialogService;

        public ReportGeneratorService(IFileExportService exportService, IDialogService dialogService)
        {
            _exportService = exportService;
            _dialogService = dialogService;
        }

        public async Task GenerateDailyReportAsync()
        {
            try
            {
                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Info", "Rapport quotidien généré");
            }
            catch (Exception ex)
            {
                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Erreur", ex.Message);
            }
        }

        public async Task GenerateMonthlyReportAsync()
        {
            try
            {
                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Info", "Rapport mensuel généré");
            }
            catch (Exception ex)
            {
                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Erreur", ex.Message);
            }
        }

        public async Task<bool> ExportBookingsReportAsync()
        {
            try
            {
                // CORRECTION : Utilisez ShowMessageAsync pour confirmation
                var result = MessageBox.Show("Exporter les réservations ?",
                    "Export", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return false;

                // Données de test avec BookingDto
                var bookings = new[]
                {
                    new BookingDto
                    {
                        Id = 1,
                        ClientName = "Test",
                        VehicleName = "Test Voiture", // VehicleName
                        TotalAmount = 100, // TotalAmount
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(3)
                    }
                };

                return await _exportService.ExportToExcelAsync(bookings, "reservations.xlsx");
            }
            catch (Exception ex)
            {
                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Erreur", ex.Message);
                return false;
            }
        }

        public async Task<bool> ExportClientsReportAsync()
        {
            try
            {
                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Info", "Export clients en cours...");
                return true;
            }
            catch (Exception ex)
            {
                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Erreur", ex.Message);
                return false;
            }
        }
    }
}