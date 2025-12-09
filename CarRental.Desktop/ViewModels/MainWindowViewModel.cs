using CarRental.Application.DTOs;
using CarRental.Desktop.Services;
using CarRental.Desktop.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels
{
    /// <summary>
    /// ViewModel principal de la fenêtre principale
    /// Gère la navigation entre les différents modules et affiche le module actuel
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IBookingServiceClient _bookingService;
        private readonly IDialogService _dialogService;
        private readonly IAuthenticationService _authService;

        private string _message = "Bienvenue dans CarRental Desktop";
        private ObservableCollection<BookingDto> _recentBookings = new();
        private ViewModelBase? _currentViewModel;
        private string _currentPageTitle = "Dashboard";

        public MainWindowViewModel(
            INavigationService navigationService,
            IBookingServiceClient bookingService,
            IDialogService dialogService,
            IAuthenticationService authService)
        {
            _navigationService = navigationService;
            _bookingService = bookingService;
            _dialogService = dialogService;
            _authService = authService;

            // S'abonner aux changements de navigation
            _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;

            // Créer les commandes de navigation
            NavigateToDashboardCommand = new RelayCommand(_ => NavigateToDashboard());
            NavigateToClientsCommand = new RelayCommand(_ => NavigateToClients());
            NavigateToVehiclesCommand = new RelayCommand(_ => NavigateToVehicles());
            NavigateToVehicleTypesCommand = new RelayCommand(_ => NavigateToVehicleTypes());
            NavigateToBookingsCommand = new RelayCommand(_ => NavigateToBookings());
            NavigateToRentalsCommand = new RelayCommand(_ => NavigateToBookings());
            NavigateToEmployeesCommand = new RelayCommand(_ => NavigateToEmployees());
            NavigateToAvailabilityCommand = new RelayCommand(_ => NavigateToAvailability());
            NavigateToPaymentsCommand = new RelayCommand(_ => NavigateToPayments());
            NavigateToAlertsCommand = new RelayCommand(_ => NavigateToAlerts());
            NavigateToReportsCommand = new RelayCommand(_ => NavigateToReports());
            NavigateToSettingsCommand = new RelayCommand(_ => NavigateToSettings());
            NavigateToLoginCommand = new AsyncRelayCommand(LogoutAsync);
            LogoutCommand = new AsyncRelayCommand(LogoutAsync);
            RefreshCommand = new AsyncRelayCommand(LoadRecentBookingsAsync);

            // Charger les données initiales
            _ = LoadRecentBookingsAsync();

            // Naviguer vers le Dashboard par défaut
            NavigateToDashboard();
        }

        #region Propriétés

        /// <summary>
        /// Message de bienvenue affiché en haut
        /// </summary>
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        /// <summary>
        /// Liste des réservations récentes (sidebar ou widget)
        /// </summary>
        public ObservableCollection<BookingDto> RecentBookings
        {
            get => _recentBookings;
            set => SetProperty(ref _recentBookings, value);
        }

        /// <summary>
        /// ViewModel actuellement affiché dans le contenu principal
        /// </summary>
        public ViewModelBase? CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        /// <summary>
        /// Titre de la page actuelle (pour le header)
        /// </summary>
        public string CurrentPageTitle
        {
            get => _currentPageTitle;
            set => SetProperty(ref _currentPageTitle, value);
        }

        /// <summary>
        /// Nom de l'utilisateur connecté
        /// </summary>
        public string CurrentUserName => SessionManager.CurrentUsername ?? "Utilisateur";

        /// <summary>
        /// Rôle de l'utilisateur
        /// </summary>
        public string CurrentUserRole => SessionManager.CurrentUserRole ?? "Agent";

        /// <summary>
        /// Indique si l'utilisateur est connecté
        /// </summary>
        public bool IsLoggedIn => SessionManager.IsLoggedIn;

        /// <summary>
        /// Indique si l'utilisateur est Admin (pour afficher/masquer certains menus)
        /// </summary>
        public bool IsAdmin => SessionManager.CurrentUserRole == "Admin";

        #endregion

        #region Commandes de Navigation

        public ICommand NavigateToDashboardCommand { get; }
        public ICommand NavigateToClientsCommand { get; }
        public ICommand NavigateToVehiclesCommand { get; }
        public ICommand NavigateToVehicleTypesCommand { get; }
        public ICommand NavigateToBookingsCommand { get; }
        public ICommand NavigateToRentalsCommand { get; }
        public ICommand NavigateToEmployeesCommand { get; }
        public ICommand NavigateToAvailabilityCommand { get; }
        public ICommand NavigateToPaymentsCommand { get; }
        public ICommand NavigateToAlertsCommand { get; }
        public ICommand NavigateToReportsCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }
        public ICommand NavigateToLoginCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand RefreshCommand { get; }

        #endregion

        #region Méthodes de Navigation

        private void NavigateToDashboard()
        {
            CurrentPageTitle = "Tableau de Bord";
            _navigationService.NavigateTo<DashboardViewModel>();
        }

        private void NavigateToClients()
        {
            CurrentPageTitle = "Gestion des Clients";
            _navigationService.NavigateTo<ClientsViewModel>();
        }

        private void NavigateToVehicles()
        {
            CurrentPageTitle = "Gestion des Véhicules";
            _navigationService.NavigateTo<VehiclesViewModel>();
        }

        private void NavigateToVehicleTypes()
        {
            CurrentPageTitle = "Types de Véhicules";
            _navigationService.NavigateTo<VehicleTypesViewModel>();
        }

        private void NavigateToBookings()
        {
            CurrentPageTitle = "Gestion des Réservations";
            _navigationService.NavigateTo<RentalsViewModel>();
        }

        private void NavigateToEmployees()
        {
            CurrentPageTitle = "Gestion des Employés";
            _navigationService.NavigateTo<EmployeesViewModel>();
        }

        private void NavigateToAvailability()
        {
            CurrentPageTitle = "Disponibilité";
            _navigationService.NavigateTo<AvailabilityViewModel>();
        }

        private void NavigateToPayments()
        {
            CurrentPageTitle = "Gestion des Paiements";
            _navigationService.NavigateTo<PaymentsViewModel>();
        }

        private void NavigateToAlerts()
        {
            CurrentPageTitle = "Alertes";
            _navigationService.NavigateTo<AlertsViewModel>();
        }

        private void NavigateToReports()
        {
            CurrentPageTitle = "Rapports et Exports";
            _navigationService.NavigateTo<ReportViewModel>();
        }

        private void NavigateToSettings()
        {
            CurrentPageTitle = "Paramètres";
            _navigationService.NavigateTo<SettingsViewModel>();
        }

        /// <summary>
        /// Appelé quand le ViewModel actuel change via le NavigationService
        /// </summary>
        private void OnCurrentViewModelChanged(ViewModelBase? viewModel)
        {
            CurrentViewModel = viewModel;
        }

        #endregion

        #region Chargement des Données

        /// <summary>
        /// Charge les réservations récentes pour la sidebar
        /// </summary>
        private async Task LoadRecentBookingsAsync()
        {
            try
            {
                IsLoading = true;
                ClearError();

                var dashboard = await _bookingService.GetDashboardDataAsync();

                if (dashboard?.RecentBookings != null)
                {
                    RecentBookings = new ObservableCollection<BookingDto>(dashboard.RecentBookings);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du chargement : {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Erreur LoadRecentBookings: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Déconnexion

        /// <summary>
        /// Déconnecte l'utilisateur
        /// </summary>
        private async Task LogoutAsync()
        {
            var confirmed = await _dialogService.ShowConfirmationAsync(
                "Confirmation",
                "Voulez-vous vraiment vous déconnecter ?");

            if (confirmed)
            {
                _authService.Logout();

                await _dialogService.ShowMessageAsync("Info", "Vous êtes déconnecté");
                
                // Navigate back to Login
                _navigationService.NavigateTo<LoginViewModel>();
            }
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Nettoyer les ressources quand la fenêtre se ferme
        /// </summary>
        public void OnClosing()
        {
            _navigationService.CurrentViewModelChanged -= OnCurrentViewModelChanged;
        }

        #endregion
    }
}