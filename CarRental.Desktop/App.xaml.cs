using CarRental.Application.Interfaces; // Assure-toi d'avoir les interfaces ici
using CarRental.Desktop.Services;
using CarRental.Desktop.ViewModels;
using CarRental.Desktop.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace CarRental.Desktop
{
    public partial class App : System.Windows.Application
    {
        public IServiceProvider Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IFileExportService, FileExportService>();
            services.AddSingleton<IDataImportService, DataImportService>();
            services.AddSingleton<IEmailNotificationService, EmailNotificationService>();
            services.AddSingleton<IPrintService, PrintService>();
            services.AddSingleton<IReportGeneratorService, ReportGeneratorService>();

            // --- 2. GESTION DE L'ÉTAT ---
            services.AddSingleton<AppState>();

            // --- 3. MOTEUR DE NAVIGATION ---
            // Factory pour créer les ViewModels à la volée
            services.AddSingleton<Func<Type, ViewModelBase>>(provider =>
                viewModelType => (ViewModelBase)provider.GetRequiredService(viewModelType));

            // --- 4. SERVICES MÉTIERS (MOCKS TEMPORAIRES) ---
            // IMPORTANT : Tes ViewModels ont besoin de ces interfaces dans leurs constructeurs.
            // On enregistre tes classes "Client" qui contiennent pour l'instant les fausses données.

            services.AddSingleton<IBookingServiceClient, BookingServiceClient>();

            // Pour IAuthService, IClientService, etc., si tu n'as pas encore les classes,
            // tu dois soit créer des mocks vides, soit utiliser BookingServiceClient s'il fait tout pour l'instant.
            // EXEMPLE (à décommenter quand tu auras créé les classes Mocks) :
            // services.AddSingleton<IClientService, ClientServiceMock>();
            // services.AddSingleton<IVehicleService, VehicleServiceMock>();
            // services.AddSingleton<IAuthService, AuthServiceMock>();

            // --- 5. VIEWMODELS ---
            services.AddSingleton<MainWindowViewModel>();

            services.AddTransient<LoginViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<ClientManagementViewModel>();
            services.AddTransient<VehicleManagementViewModel>();
            services.AddTransient<BookingManagementViewModel>();
            services.AddTransient<PaymentManagementViewModel>();
            services.AddTransient<MaintenanceViewModel>();
            services.AddTransient<ReportViewModel>();
            services.AddTransient<SettingsViewModel>();

            // --- 6. FENÊTRES (VIEWS) ---
            // On enregistre MainWindow pour pouvoir lui injecter des choses si besoin plus tard
            services.AddSingleton<MainWindow>();

            // --- CONSTRUCTION ---
            Services = services.BuildServiceProvider();

            // Initialisation du ServiceLocator (pour les cas où l'injection n'est pas possible)
            ServiceLocator.Initialize(Services);

            // --- DÉMARRAGE ---
            // On récupère la MainWindow depuis le DI (plus propre que 'new MainWindow()')
            var mainWindow = Services.GetRequiredService<MainWindow>();

            // Assignation du DataContext
            mainWindow.DataContext = Services.GetRequiredService<MainWindowViewModel>();

            mainWindow.Show();
        }
    }
}