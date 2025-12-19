using System;
using System.IO;
using System.Windows;
using CarRental.Desktop.Services;
using CarRental.Desktop.ViewModels;

namespace CarRental.Desktop
{
    public partial class App : System.Windows.Application
    {
        private NavigationService? _navigationService;

        // Core Services
        private readonly IApiClient _apiClient;
        private readonly IAuthenticationService _authService;

        // Data Services
        private readonly IVehicleService _vehicleService;
        private readonly IRentalService _rentalService;
        private readonly IClientService _clientService;
        private readonly IEmployeeService _employeeService;
        private readonly IVehicleTypeService _vehicleTypeService;
        private readonly IBookingServiceClient _bookingService;
        private readonly IDashboardService _dashboardService;
        private readonly IPaymentService _paymentService;

        // UI/Utility Services
        private readonly IDialogService _dialogService;
        private readonly IPrintService _printService;
        private readonly IFileExportService _fileExportService;

        public App()
        {
            // Set up global exception handling
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // 1. Load Configuration
            var config = Configuration.ConfigurationLoader.Load();

            // Initialize UI/Utility Services first so ApiClient can show errors
            _dialogService = new DialogService();
            _printService = new PrintService(_dialogService);
            _fileExportService = new FileExportService(_dialogService);

            // Initialize API client and core services
            string baseUrl = !string.IsNullOrEmpty(config.ApiBaseUrl) ? config.ApiBaseUrl : "http://localhost:5120";
            _apiClient = new ApiClient(baseUrl, _dialogService);
            _authService = new AuthenticationService(_apiClient);
            _dashboardService = new ApiDashboardService(_apiClient);
            _paymentService = new ApiPaymentService(_apiClient);

            // Initialize data services depending on mock flag
            if (config.UseMockServices)
            {
                _vehicleService = new MockVehicleService();
                _rentalService = new ApiRentalService(_apiClient);
                _clientService = new ApiClientService(_apiClient);
                _employeeService = new ApiEmployeeService(_apiClient);
                _vehicleTypeService = new ApiVehicleTypeService(_apiClient);
                _bookingService = new BookingServiceClient();
                // Mock PaymentService not implemented, falling back to API or null? 
                // Let's assume API for now as there's no MockPaymentService requested
            }
            else
            {
                _vehicleService = new ApiVehicleService(_apiClient);
                _clientService = new ApiClientService(_apiClient);
                _rentalService = new ApiRentalService(_apiClient);
                _employeeService = new ApiEmployeeService(_apiClient);
                _vehicleTypeService = new ApiVehicleTypeService(_apiClient);
                _bookingService = new ApiBookingService(_apiClient);
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogException(e.Exception, "DispatcherUnhandledException");
            e.Handled = true; // Prevent crash if possible, or at least log it
            MessageBox.Show($"An unhandled exception occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogException(e.ExceptionObject as Exception, "CurrentDomain_UnhandledException");
        }

        private void LogException(Exception? ex, string source)
        {
            if (ex == null) return;
            try
            {
                string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "desktop_crash.log");
                string message = $"{DateTime.Now}: [{source}] {ex.Message}\nStack Trace: {ex.StackTrace}\n\n";
                File.AppendAllText(logFile, message);
                
                // Also try to log to our FileLogger if possible, but manual append is safer for crashes
            }
            catch { /* Ignore logging errors during crash */ }
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            CarRental.Desktop.Services.FileLogger.Log("Application Starting...");
            base.OnStartup(e);

            _navigationService = new NavigationService(CreateViewModel);

            var mainViewModel = new MainWindowViewModel(_navigationService, _bookingService, _dialogService, _authService);

            // Set initial view
            _navigationService.NavigateTo<LoginViewModel>();

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            mainWindow.Show();
            
            mainWindow.Closed += (s, args) => CarRental.Desktop.Services.FileLogger.Log("Application Closed.");
        }

        private ViewModelBase CreateViewModel(Type viewModelType)
        {
            if (viewModelType == typeof(LoginViewModel))
                return new LoginViewModel(_navigationService!, _authService);

            if (viewModelType == typeof(DashboardViewModel))
                return new DashboardViewModel(_dashboardService);

            if (viewModelType == typeof(VehiclesViewModel))
                return new VehiclesViewModel(_vehicleService);

            if (viewModelType == typeof(VehicleTypesViewModel))
                return new VehicleTypesViewModel(_vehicleTypeService);

            if (viewModelType == typeof(VehicleManagementViewModel))
                return new VehicleManagementViewModel(_dialogService);

            if (viewModelType == typeof(AvailabilityViewModel))
                return new AvailabilityViewModel(_vehicleService);

            if (viewModelType == typeof(ClientsViewModel))
                return new ClientsViewModel(_clientService);

            if (viewModelType == typeof(ClientManagementViewModel))
                return new ClientManagementViewModel(_dialogService);

            if (viewModelType == typeof(EmployeesViewModel))
                return new EmployeesViewModel(_employeeService);

            if (viewModelType == typeof(RentalsViewModel))
                return new RentalsViewModel(_rentalService, _clientService, _vehicleService, _dialogService);

            if (viewModelType == typeof(BookingManagementViewModel))
                return new BookingManagementViewModel(_bookingService, _printService, _dialogService);

            if (viewModelType == typeof(PaymentsViewModel))
                return new PaymentsViewModel(_paymentService);

            if (viewModelType == typeof(PaymentManagementViewModel))
                return new PaymentManagementViewModel(_dialogService);

            if (viewModelType == typeof(MaintenanceViewModel))
                return new MaintenanceViewModel(_dialogService);

            if (viewModelType == typeof(AlertsViewModel))
                return new AlertsViewModel(_dashboardService);

            if (viewModelType == typeof(ReportViewModel))
                return new ReportViewModel(_fileExportService, _dialogService);

            if (viewModelType == typeof(SettingsViewModel))
                return new SettingsViewModel();

            throw new ArgumentException($"ViewModel of type {viewModelType.Name} not registered in App.xaml.cs.");
        }
    }
}
