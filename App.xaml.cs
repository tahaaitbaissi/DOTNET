using System;
using System.Windows;
using CarRental.Desktop.Services;
using CarRental.Desktop.ViewModels;

namespace CarRental.Desktop
{
    public partial class App : Application
    {
        private NavigationService _navigationService;

        private readonly Services.IVehicleService _vehicleService;
        private readonly Services.IRentalService _rentalService;
        private readonly Services.IClientService _clientService;
        private readonly Services.IEmployeeService _employeeService;
        private readonly Services.IVehicleTypeService _vehicleTypeService;

        public App()
        {
            // Simple manual dependency injection setup
            // 1. Load Configuration
            var config = Configuration.ConfigurationLoader.Load();

            // 2. Initialize Services based on config
            if (config.UseMockServices)
            {
                _vehicleService = new Services.MockVehicleService();
                _rentalService = new Services.MockRentalService();
                _clientService = new Services.MockClientService();
                _employeeService = new Services.MockEmployeeService();
                _vehicleTypeService = new Services.MockVehicleTypeService();
            }
            else
            {
                // TODO: Initialize Real Services here using config.ApiBaseUrl
                // _vehicleService = new Services.ApiVehicleService(config.ApiBaseUrl);
                throw new NotImplementedException("Real services are not yet implemented. Please implement them and update App.xaml.cs");
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _navigationService = new NavigationService(CreateViewModel);

            var mainViewModel = new MainViewModel(_navigationService);
            
            // Set initial view
            _navigationService.NavigateTo<DashboardViewModel>();

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
            mainWindow.Show();
        }

        private ViewModelBase CreateViewModel(Type viewModelType)
        {
            if (viewModelType == typeof(DashboardViewModel)) return new DashboardViewModel(_vehicleService, _rentalService);
            if (viewModelType == typeof(LoginViewModel)) return new LoginViewModel(_navigationService);
            if (viewModelType == typeof(VehiclesViewModel)) return new VehiclesViewModel(_vehicleService);
            if (viewModelType == typeof(RentalsViewModel)) return new RentalsViewModel(_rentalService);
            if (viewModelType == typeof(ClientsViewModel)) return new ClientsViewModel(_clientService);
            if (viewModelType == typeof(EmployeesViewModel)) return new EmployeesViewModel(_employeeService);
            if (viewModelType == typeof(AvailabilityViewModel)) return new AvailabilityViewModel(_vehicleService);
            if (viewModelType == typeof(PaymentsViewModel)) return new PaymentsViewModel();
            if (viewModelType == typeof(AlertsViewModel)) return new AlertsViewModel();
            if (viewModelType == typeof(VehicleTypesViewModel)) return new VehicleTypesViewModel(_vehicleTypeService);
            
            throw new ArgumentException($"ViewModel of type {viewModelType.Name} not registered.");
        }
    }
}
