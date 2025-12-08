using CarRental.Core.Entities;
using CarRental.Desktop.Services;
using CarRental.Desktop.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels
{
    public class MaintenanceViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;

        private ObservableCollection<Maintenance> _activeMaintenances = new();
        private ObservableCollection<Notification> _alerts = new();

        public MaintenanceViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            LoadMaintenancesCommand = new AsyncRelayCommand(LoadMaintenancesAsync);
            LoadAlertsCommand = new AsyncRelayCommand(LoadAlertsAsync);
            ScheduleMaintenanceCommand = new AsyncRelayCommand(ScheduleMaintenanceAsync);
            CheckAlertsCommand = new AsyncRelayCommand(CheckAlertsAsync);
            RefreshCommand = new AsyncRelayCommand(RefreshAllAsync);

            _ = LoadMaintenancesAsync();
            _ = LoadAlertsAsync();
        }

        public ObservableCollection<Maintenance> ActiveMaintenances
        {
            get => _activeMaintenances;
            set => SetProperty(ref _activeMaintenances, value);
        }

        public ObservableCollection<Notification> Alerts
        {
            get => _alerts;
            set => SetProperty(ref _alerts, value);
        }

        public ICommand LoadMaintenancesCommand { get; }
        public ICommand LoadAlertsCommand { get; }
        public ICommand ScheduleMaintenanceCommand { get; }
        public ICommand CheckAlertsCommand { get; }
        public ICommand RefreshCommand { get; }

        private async Task LoadMaintenancesAsync()
        {
            try
            {
                IsLoading = true;
                ClearError();
                await Task.Delay(300);

                var mockMaintenances = new ObservableCollection<Maintenance>();

                var maintenance1 = new Maintenance
                {
                    VehicleId = 1,
                    Type = "Routine",
                    StartDate = DateTime.Now,
                    Details = "Vidange",
                    Cost = 150.00m
                };

                var maintenance2 = new Maintenance
                {
                    VehicleId = 2,
                    Type = "Repair",
                    StartDate = DateTime.Now.AddDays(-5),
                    Details = "Réparation freins",
                    Cost = 350.00m
                };

                mockMaintenances.Add(maintenance1);
                mockMaintenances.Add(maintenance2);

                ActiveMaintenances = mockMaintenances;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await _dialogService.ShowErrorAsync("Erreur", ErrorMessage);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadAlertsAsync()
        {
            try
            {
                await Task.Delay(200);

                var mockAlerts = new ObservableCollection<Notification>();

                var alert1 = new Notification
                {
                    Title = "Maintenance requise",
                    Body = "Véhicule #1 nécessite une révision",
                    Type = "Warning",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                var alert2 = new Notification
                {
                    Title = "Assurance à renouveler",
                    Body = "Véhicule #3 - Assurance expire dans 30 jours",
                    Type = "Info",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                mockAlerts.Add(alert1);
                mockAlerts.Add(alert2);

                Alerts = mockAlerts;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Erreur", ex.Message);
            }
        }

        private async Task ScheduleMaintenanceAsync()
        {
            await _dialogService.ShowInfoAsync("Info", "Planification de maintenance (non implémenté)");
        }

        private async Task CheckAlertsAsync()
        {
            await LoadAlertsAsync();
            await _dialogService.ShowInfoAsync("Info", $"{Alerts.Count} alertes trouvées");
        }

        private async Task RefreshAllAsync()
        {
            await LoadMaintenancesAsync();
            await LoadAlertsAsync();
        }
    }
}