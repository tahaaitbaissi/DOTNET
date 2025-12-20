using System.Collections.ObjectModel;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class AlertsViewModel : ViewModelBase
    {
        private readonly IDashboardService _dashboardService;
        private ObservableCollection<string> _alerts;

        public ObservableCollection<string> Alerts
        {
             get => _alerts;
             set => SetProperty(ref _alerts, value);
        }

        public AlertsViewModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
            Alerts = new ObservableCollection<string>();
            
            Title = "Alerts & Notifications";
            _ = LoadAlertsAsync();
        }

        private async Task LoadAlertsAsync()
        {
            IsLoading = true;
            var result = await _dashboardService.GetDashboardDataAsync();
            
            if (result.IsSuccess && result.Value.Alerts != null && result.Value.Alerts.Any())
            {
                Alerts = new ObservableCollection<string>(result.Value.Alerts);
            }
            else
            {
                Alerts = new ObservableCollection<string> { "No active alerts." };
            }
            IsLoading = false;
        }
    }
}
