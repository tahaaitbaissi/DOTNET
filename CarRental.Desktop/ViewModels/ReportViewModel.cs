using CarRental.Desktop.Services;
using CarRental.Desktop.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels
{
    public class ReportViewModel : ViewModelBase
    {
        private readonly IFileExportService _exportService;
        private readonly IDialogService _dialogService;

        public ReportViewModel(IFileExportService exportService, IDialogService dialogService)
        {
            _exportService = exportService;
            _dialogService = dialogService;

            // ✅ CORRIGÉ: AsyncRelayCommand
            ExportClientsCommand = new AsyncRelayCommand(ExportClientsAsync);
            ExportVehiclesCommand = new AsyncRelayCommand(ExportVehiclesAsync);
            ExportBookingsCommand = new AsyncRelayCommand(ExportBookingsAsync);
        }

        public ICommand ExportClientsCommand { get; }
        public ICommand ExportVehiclesCommand { get; }
        public ICommand ExportBookingsCommand { get; }

        private async Task ExportClientsAsync()
        {
            try
            {
                IsLoading = true;
                await _dialogService.ShowMessageAsync("Info", "Export clients en cours...");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExportVehiclesAsync()
        {
            try
            {
                IsLoading = true;
                await _dialogService.ShowMessageAsync("Info", "Export véhicules en cours...");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExportBookingsAsync()
        {
            try
            {
                IsLoading = true;
                await _dialogService.ShowMessageAsync("Info", "Export réservations en cours...");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}