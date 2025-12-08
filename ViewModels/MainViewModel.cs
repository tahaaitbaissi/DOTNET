using System.Windows.Input;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public ViewModelBase CurrentView => _navigationService.CurrentView;

        public ICommand NavigateToDashboardCommand { get; }
        public ICommand NavigateToVehiclesCommand { get; }
        public ICommand NavigateToClientsCommand { get; }
        public ICommand NavigateToRentalsCommand { get; }
        public ICommand NavigateToEmployeesCommand { get; }
        public ICommand NavigateToAvailabilityCommand { get; }
        public ICommand NavigateToPaymentsCommand { get; }
        public ICommand NavigateToAlertsCommand { get; }
        public ICommand NavigateToVehicleTypesCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _navigationService.CurrentViewChanged += OnCurrentViewChanged;

            NavigateToDashboardCommand = new RelayCommand(o => _navigationService.NavigateTo<DashboardViewModel>());
            NavigateToVehiclesCommand = new RelayCommand(o => _navigationService.NavigateTo<VehiclesViewModel>());
            NavigateToClientsCommand = new RelayCommand(o => _navigationService.NavigateTo<ClientsViewModel>());
            NavigateToRentalsCommand = new RelayCommand(o => _navigationService.NavigateTo<RentalsViewModel>());
            NavigateToEmployeesCommand = new RelayCommand(o => _navigationService.NavigateTo<EmployeesViewModel>());
            NavigateToAvailabilityCommand = new RelayCommand(o => _navigationService.NavigateTo<AvailabilityViewModel>());
            NavigateToPaymentsCommand = new RelayCommand(o => _navigationService.NavigateTo<PaymentsViewModel>());
            NavigateToAlertsCommand = new RelayCommand(o => _navigationService.NavigateTo<AlertsViewModel>());
            NavigateToLoginCommand = new RelayCommand(o => _navigationService.NavigateTo<LoginViewModel>());
            NavigateToVehicleTypesCommand = new RelayCommand(o => _navigationService.NavigateTo<VehicleTypesViewModel>());
        }

        private void OnCurrentViewChanged()
        {
            OnPropertyChanged(nameof(CurrentView));
        }
    }
}
