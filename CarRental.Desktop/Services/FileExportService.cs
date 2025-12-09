using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Desktop.Services;

public class FileExportService : IFileExportService
{
    private readonly IDialogService _dialogService;

    public FileExportService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async Task<bool> ExportToExcelAsync<T>(IEnumerable<T> data, string filePath, string sheetName = "Data")
    {
        try
        {
            // TODO: Implémenter avec ClosedXML
            await Task.Delay(100); // Simulation
            await _dialogService.ShowInfoAsync("Export", "Fichier exporté");
            return true;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Erreur", ex.Message);
            return false;
        }
    }

    public Task<bool> ExportClientsAsync(IEnumerable<ClientDto> clients, string filePath)
    {
        return ExportToExcelAsync(clients, filePath, "Clients");
    }

    public Task<bool> ExportVehiclesAsync(IEnumerable<VehicleDto> vehicles, string filePath)
    {
        return ExportToExcelAsync(vehicles, filePath, "Véhicules");
    }

    public Task<bool> ExportBookingsAsync(IEnumerable<BookingDto> bookings, string filePath)
    {
        return ExportToExcelAsync(bookings, filePath, "Réservations");
    }
}