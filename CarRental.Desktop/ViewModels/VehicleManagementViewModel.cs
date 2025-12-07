using CarRental.Application.DTOs;
using CarRental.Desktop.Services;
using CarRental.Desktop.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels
{
    public class VehicleManagementViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;

        private ObservableCollection<VehicleDto> _vehicles = new();
        private VehicleDto? _selectedVehicle;

        public VehicleManagementViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            // ✅ CORRIGÉ: AsyncRelayCommand
            LoadVehiclesCommand = new AsyncRelayCommand(LoadVehiclesAsync);
            AddVehicleCommand = new AsyncRelayCommand(AddVehicleAsync);
            UpdateVehicleCommand = new AsyncRelayCommand(UpdateVehicleAsync, CanUpdateVehicle);
            DeleteVehicleCommand = new AsyncRelayCommand(DeleteVehicleAsync, CanDeleteVehicle);
            RefreshCommand = new AsyncRelayCommand(LoadVehiclesAsync);

            _ = LoadVehiclesAsync();
        }

        public ObservableCollection<VehicleDto> Vehicles
        {
            get => _vehicles;
            set => SetProperty(ref _vehicles, value);
        }

        public VehicleDto? SelectedVehicle
        {
            get => _selectedVehicle;
            set => SetProperty(ref _selectedVehicle, value);
        }

        public ICommand LoadVehiclesCommand { get; }
        public ICommand AddVehicleCommand { get; }
        public ICommand UpdateVehicleCommand { get; }
        public ICommand DeleteVehicleCommand { get; }
        public ICommand RefreshCommand { get; }

        private bool CanUpdateVehicle() => SelectedVehicle != null;
        private bool CanDeleteVehicle() => SelectedVehicle != null;

        private async Task LoadVehiclesAsync()
        {
            try
            {
                IsLoading = true;
                ClearError();

                // Données MOCK
                await Task.Delay(300);
                var mockVehicles = new[]
                {
                    new VehicleDto { Id = 1, Make = "Toyota", Model = "Corolla", Year = 2023, Color = "Blanc", LicensePlate = "AB-123-CD", Status = "Available", DailyRate = 50 },
                    new VehicleDto { Id = 2, Make = "Renault", Model = "Clio", Year = 2022, Color = "Rouge", LicensePlate = "EF-456-GH", Status = "Rented", DailyRate = 45 },
                    new VehicleDto { Id = 3, Make = "Peugeot", Model = "308", Year = 2024, Color = "Noir", LicensePlate = "IJ-789-KL", Status = "Available", DailyRate = 55 }
                };

                Vehicles = new ObservableCollection<VehicleDto>(mockVehicles);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await _dialogService.ShowMessageAsync("Erreur", ErrorMessage);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddVehicleAsync()
        {
            await _dialogService.ShowMessageAsync("Info", "Ajout de véhicule (non implémenté)");
        }

        private async Task UpdateVehicleAsync()
        {
            if (SelectedVehicle == null) return;
            await _dialogService.ShowMessageAsync("Info", $"Modification de {SelectedVehicle.DisplayName}");
        }

        private async Task DeleteVehicleAsync()
        {
            if (SelectedVehicle == null) return;

            var confirmed = await _dialogService.ShowConfirmationAsync(
                "Confirmation",
                $"Supprimer {SelectedVehicle.DisplayName} ?");

            if (confirmed)
            {
                Vehicles.Remove(SelectedVehicle);
                await _dialogService.ShowMessageAsync("Succès", "Véhicule supprimé");
            }
        }
    }
}