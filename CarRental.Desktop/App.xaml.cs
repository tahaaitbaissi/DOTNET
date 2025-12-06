using CarRental.Desktop.Services;
using CarRental.Desktop.ViewModels;
using CarRental.Desktop.ViewModels.Base; // N'oublie pas ce namespace
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace CarRental.Desktop // Assure-toi du namespace
{
    public partial class App : System.Windows.Application
    {
        // On expose les services si besoin pour le debug
        public IServiceProvider Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // --- 1. SERVICES D'INFRASTRUCTURE (Toi) ---
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IFileExportService, FileExportService>();

            // Ajout des services manquants vus dans ton arborescence
            services.AddSingleton<IDataImportService, DataImportService>();
            services.AddSingleton<IEmailNotificationService, EmailNotificationService>();
            services.AddSingleton<IPrintService, PrintService>();
            services.AddSingleton<IReportGeneratorService, ReportGeneratorService>();

            // --- 2. GESTION DE L'ÉTAT (CRUCIAL) ---
            // Sans ça, le LoginViewModel ne pourra pas stocker l'utilisateur
            services.AddSingleton<SessionManager>();
            services.AddSingleton<AppState>();

            // --- 3. MOTEUR DE NAVIGATION (CRUCIAL) ---
            // Cette ligne permet au NavigationService de créer n'importe quel ViewModel demandé
            services.AddSingleton<Func<Type, ViewModelBase>>(provider =>
                viewModelType => (ViewModelBase)provider.GetRequiredService(viewModelType));

            // --- 4. SERVICES BACKEND (Membre 4) ---
            // Mock ou implémentation réelle nécessaire pour que LoginViewModel ne plante pas
            // Si tu n'as pas encore le code de Membre 4, commente ces lignes, 
            // MAIS tes ViewModels devront gérer l'absence de service (null check)
            services.AddHttpClient();
            // services.AddScoped<IClientService, ClientService>();
            // services.AddScoped<IAuthService, AuthService>(); 

            // --- 5. ENREGISTREMENT DES VIEWMODELS ---
            services.AddSingleton<MainWindowViewModel>(); // Le chef d'orchestre

            // Les pages (Transient = nouvelle instance à chaque visite)
            services.AddTransient<LoginViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<ClientManagementViewModel>();
            services.AddTransient<VehicleManagementViewModel>();
            services.AddTransient<BookingManagementViewModel>();
            services.AddTransient<PaymentManagementViewModel>();
            services.AddTransient<MaintenanceViewModel>();
            services.AddTransient<ReportViewModel>();
            services.AddTransient<SettingsViewModel>();

            // --- CONSTRUCTION ---
            Services = services.BuildServiceProvider();

            // Initialisation du ServiceLocator
            // (Si ta classe ServiceLocator a une propriété statique 'Provider')
            ServiceLocator.Provider = Services;

            // --- DÉMARRAGE ---
            var mainWindow = new MainWindow();

            // On injecte le DataContext pour que le Binding fonctionne
            mainWindow.DataContext = Services.GetRequiredService<MainWindowViewModel>();

            mainWindow.Show();
        }
    }
}