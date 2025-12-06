using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Desktop.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly IDashboardService _dashboardService;

    private DashboardDto? _dashboardData;
    private ObservableCollection<BookingDto> _recentBookings = new();
    private bool _isLoading;

    public DashboardDto? DashboardData
    {
        get => _dashboardData;
        set
        {
            if (SetProperty(ref _dashboardData, value) && value != null)
            {
                RecentBookings = new ObservableCollection<BookingDto>(value.RecentBookings);
            }
        }
    }

    public ObservableCollection<BookingDto> RecentBookings
    {
        get => _recentBookings;
        set => SetProperty(ref _recentBookings, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    // Propriétés calculées pour binding direct
    public int AvailableVehicles => DashboardData?.AvailableVehicles ?? 0;
    public int RentedVehicles => DashboardData?.RentedVehicles ?? 0;
    public int TotalVehicles => DashboardData?.TotalVehicles ?? 0;
    public int ActiveBookings => DashboardData?.ActiveBookings ?? 0;
    public decimal TotalRevenue => DashboardData?.TotalRevenue ?? 0;
    public decimal MonthlyRevenue => DashboardData?.MonthlyRevenue ?? 0;

    public ICommand LoadDashboardCommand { get; }
    public ICommand RefreshCommand { get; }

    public DashboardViewModel(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;

        LoadDashboardCommand = new RelayCommand(async (param) => await LoadDashboardAsync());
        RefreshCommand = new RelayCommand(async (param) => await RefreshDashboardAsync());

        LoadDashboardCommand.Execute(null);
    }

    private async Task LoadDashboardAsync()
    {
        IsLoading = true;
        try
        {
            var result = await _dashboardService.GetDashboardDataAsync();

            if (result.IsSuccess && result.Value != null)
            {
                DashboardData = result.Value;

                // Notifier les propriétés calculées
                OnPropertyChanged(nameof(AvailableVehicles));
                OnPropertyChanged(nameof(RentedVehicles));
                OnPropertyChanged(nameof(TotalVehicles));
                OnPropertyChanged(nameof(ActiveBookings));
                OnPropertyChanged(nameof(TotalRevenue));
                OnPropertyChanged(nameof(MonthlyRevenue));
            }
            else
            {
                Console.WriteLine($"Erreur dashboard: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception dashboard: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RefreshDashboardAsync()
    {
        await LoadDashboardAsync();
    }
}