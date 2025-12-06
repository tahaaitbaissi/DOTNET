using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Core.Entities;
using CarRental.Desktop.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels;

public class MaintenanceViewModel : ViewModelBase
{
    private readonly IMaintenanceService _maintenanceService;

    private ObservableCollection<Maintenance> _activeMaintenances = new();
    private ObservableCollection<Notification> _alerts = new();
    private bool _isLoading;

    public ObservableCollection<Maintenance> ActiveMaintenances
    {
        get => _activeMaintenances;
        set => SetProperty(ref _activeMaintenances, value);
    }

    public ObservableCollection<Notification> Alerts
    {
        get => _alerts;
        set => SetProperty(ref _alerts, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadMaintenancesCommand { get; }
    public ICommand LoadAlertsCommand { get; }
    public ICommand ScheduleMaintenanceCommand { get; }
    public ICommand CheckAlertsCommand { get; }
    public ICommand RefreshCommand { get; }

    public MaintenanceViewModel(IMaintenanceService maintenanceService)
    {
        _maintenanceService = maintenanceService;

        LoadMaintenancesCommand = new RelayCommand(async (param) => await LoadMaintenancesAsync());
        LoadAlertsCommand = new RelayCommand(async (param) => await LoadAlertsAsync());
        ScheduleMaintenanceCommand = new RelayCommand(async (param) => await ScheduleMaintenanceAsync());
        CheckAlertsCommand = new RelayCommand(async (param) => await CheckAlertsAsync());
        RefreshCommand = new RelayCommand(async (param) => await RefreshAllAsync());

        LoadMaintenancesCommand.Execute(null);
        LoadAlertsCommand.Execute(null);
    }

    private async Task LoadMaintenancesAsync()
    {
        IsLoading = true;
        try
        {
            var result = await _maintenanceService.GetActiveMaintenancesAsync();

            if (result.IsSuccess && result.Value != null)
            {
                ActiveMaintenances = new ObservableCollection<Maintenance>(result.Value);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadAlertsAsync()
    {
        var result = await _maintenanceService.GetAdminAlertsAsync();

        if (result.IsSuccess && result.Value != null)
        {
            Alerts = new ObservableCollection<Notification>(result.Value);
        }
    }

    private async Task ScheduleMaintenanceAsync()
    {
        // TODO: Implémenter boîte de dialogue de planification
        // var createDto = new CreateMaintenanceDto { ... };
        // var result = await _maintenanceService.ScheduleMaintenanceAsync(createDto);

        // if (result.IsSuccess)
        // {
        //     await LoadMaintenancesAsync();
        // }
    }

    private async Task CheckAlertsAsync()
    {
        var result = await _maintenanceService.CheckAndGenerateAlertsAsync();

        if (result.IsSuccess)
        {
            await LoadAlertsAsync();
        }
    }

    private async Task RefreshAllAsync()
    {
        await LoadMaintenancesAsync();
        await LoadAlertsAsync();
    }
}