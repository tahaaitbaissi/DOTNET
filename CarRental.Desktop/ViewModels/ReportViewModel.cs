using CarRental.Application.Interfaces;
using CarRental.Desktop.ViewModels.Base;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels;

public class ReportViewModel : ViewModelBase
{
    private readonly IExportService _exportService;

    private bool _isGenerating;

    public bool IsGenerating
    {
        get => _isGenerating;
        set => SetProperty(ref _isGenerating, value);
    }

    public ICommand ExportClientsCommand { get; }
    public ICommand ExportVehiclesCommand { get; }
    public ICommand ExportBookingsCommand { get; }

    public ReportViewModel(IExportService exportService)
    {
        _exportService = exportService;

        ExportClientsCommand = new RelayCommand(async (param) => await ExportClientsAsync());
        ExportVehiclesCommand = new RelayCommand(async (param) => await ExportVehiclesAsync());
        ExportBookingsCommand = new RelayCommand(async (param) => await ExportBookingsAsync());
    }

    private async Task ExportClientsAsync()
    {
        IsGenerating = true;
        try
        {
            var fileBytes = await _exportService.ExportClientsAsync();

            if (fileBytes != null && fileBytes.Length > 0)
            {
                // TODO: Sauvegarder le fichier
                // File.WriteAllBytes("clients_export.xlsx", fileBytes);
            }
        }
        finally
        {
            IsGenerating = false;
        }
    }

    private async Task ExportVehiclesAsync()
    {
        IsGenerating = true;
        try
        {
            var fileBytes = await _exportService.ExportVehiclesAsync();

            if (fileBytes != null && fileBytes.Length > 0)
            {
                // TODO: Sauvegarder le fichier
                // File.WriteAllBytes("vehicles_export.xlsx", fileBytes);
            }
        }
        finally
        {
            IsGenerating = false;
        }
    }

    private async Task ExportBookingsAsync()
    {
        IsGenerating = true;
        try
        {
            var fileBytes = await _exportService.ExportBookingsAsync();

            if (fileBytes != null && fileBytes.Length > 0)
            {
                // TODO: Sauvegarder le fichier
                // File.WriteAllBytes("bookings_export.xlsx", fileBytes);
            }
        }
        finally
        {
            IsGenerating = false;
        }
    }
}