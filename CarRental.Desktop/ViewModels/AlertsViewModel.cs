using System.Collections.ObjectModel;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class AlertsViewModel : ViewModelBase
    {
        public ObservableCollection<string> Alerts { get; }

        public AlertsViewModel()
        {
            Alerts = new ObservableCollection<string>
            {
                "Maintenance Due: Toyota Camry (OIL CHANGE)",
                "Insurance Expiring: Ford Focus (Expires in 5 days)",
                "Overdue Return: Tesla Model 3 (Due yesterday)"
            };
        }
    }
}
