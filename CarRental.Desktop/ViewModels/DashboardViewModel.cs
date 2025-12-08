using CarRental.Application.DTOs;
using CarRental.Desktop.Services;
using CarRental.Desktop.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly IBookingServiceClient _bookingService;
        private readonly IDialogService _dialogService;

        private DashboardDto? _dashboardData;
        private ObservableCollection<BookingDto> _recentBookings = new();

        public DashboardViewModel(
            IBookingServiceClient bookingService,
            IDialogService dialogService)
        {
            _bookingService = bookingService;
            _dialogService = dialogService;

            LoadDashboardCommand = new AsyncRelayCommand(LoadDashboardAsync);
            RefreshCommand = new AsyncRelayCommand(LoadDashboardAsync);

            // Démarrage asynchrone sans bloquer le constructeur
            _ = LoadDashboardAsync();
        }

        public DashboardDto? DashboardData
        {
            get => _dashboardData;
            set
            {
                if (SetProperty(ref _dashboardData, value) && value != null)
                {
                    // Sécurité null check
                    RecentBookings = new ObservableCollection<BookingDto>(value.RecentBookings ?? new());

                    // Notification des propriétés calculées
                    OnPropertyChanged(nameof(TotalVehicles));
                    OnPropertyChanged(nameof(AvailableVehicles));
                    OnPropertyChanged(nameof(TotalRevenue));
                    OnPropertyChanged(nameof(ActiveBookings));
                }
            }
        }

        public ObservableCollection<BookingDto> RecentBookings
        {
            get => _recentBookings;
            set => SetProperty(ref _recentBookings, value);
        }

        public int TotalVehicles => DashboardData?.TotalVehicles ?? 0;
        public int AvailableVehicles => DashboardData?.AvailableVehicles ?? 0;
        public int ActiveBookings => DashboardData?.ActiveBookings ?? 0;
        public decimal TotalRevenue => DashboardData?.TotalRevenue ?? 0;

        public ICommand LoadDashboardCommand { get; }
        public ICommand RefreshCommand { get; }

        private async Task LoadDashboardAsync()
        {
            try
            {
                IsLoading = true;
                ClearError();

                DashboardData = await _bookingService.GetDashboardDataAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                // En production, on évite d'afficher une popup au chargement initial du dashboard
                // On préfère définir ErrorMessage pour l'afficher dans l'UI
                System.Diagnostics.Debug.WriteLine($"Erreur Dashboard: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}