using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Desktop.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels;

public class VehicleManagementViewModel : ViewModelBase
{
    private readonly IVehicleService _vehicleService;
    private readonly IVehicleTypeService _vehicleTypeService;

    private ObservableCollection<VehicleDto> _vehicles = new();
    private ObservableCollection<VehicleTypeDto> _vehicleTypes = new();
    private VehicleDto? _selectedVehicle;
    private bool _isLoading;

    public ObservableCollection<VehicleDto> Vehicles
    {
        get => _vehicles;
        set => SetProperty(ref _vehicles, value);
    }

    public ObservableCollection<VehicleTypeDto> VehicleTypes
    {
        get => _vehicleTypes;
        set => SetProperty(ref _vehicleTypes, value);
    }

    public VehicleDto? SelectedVehicle
    {
        get => _selectedVehicle;
        set => SetProperty(ref _selectedVehicle, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadVehiclesCommand { get; }
    public ICommand LoadVehicleTypesCommand { get; }
    public ICommand AddVehicleCommand { get; }
    public ICommand UpdateVehicleCommand { get; }
    public ICommand DeleteVehicleCommand { get; }
    public ICommand RefreshCommand { get; }

    public VehicleManagementViewModel(
        IVehicleService vehicleService,
        IVehicleTypeService vehicleTypeService)
    {
        _vehicleService = vehicleService;
        _vehicleTypeService = vehicleTypeService;

        LoadVehiclesCommand = new RelayCommand(async (param) => await LoadVehiclesAsync());
        LoadVehicleTypesCommand = new RelayCommand(async (param) => await LoadVehicleTypesAsync());
        AddVehicleCommand = new RelayCommand(async (param) => await AddVehicleAsync());
        UpdateVehicleCommand = new RelayCommand(async (param) => await UpdateVehicleAsync());
        DeleteVehicleCommand = new RelayCommand(async (param) => await DeleteVehicleAsync());
        RefreshCommand = new RelayCommand(async (param) => await RefreshAllAsync());

        LoadVehiclesCommand.Execute(null);
        LoadVehicleTypesCommand.Execute(null);
    }

    private async Task LoadVehiclesAsync()
    {
        IsLoading = true;
        try
        {
            var result = await _vehicleService.GetAllVehiclesAsync();

            if (result.IsSuccess && result.Value != null)
            {
                Vehicles = new ObservableCollection<VehicleDto>(result.Value);
            }
            else
            {
                Console.WriteLine($"Erreur véhicules: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadVehicleTypesAsync()
    {
        var result = await _vehicleTypeService.GetAllVehicleTypesAsync();

        if (result.IsSuccess && result.Value != null)
        {
            VehicleTypes = new ObservableCollection<VehicleTypeDto>(result.Value);
        }
    }

    private async Task AddVehicleAsync()
    {
        // TODO: Implémenter boîte de dialogue de création
        // var createDto = new CreateVehicleDto { ... };
        // var result = await _vehicleService.AddVehicleAsync(createDto);

        // if (result.IsSuccess)
        // {
        //     await LoadVehiclesAsync();
        // }
    }

    private async Task UpdateVehicleAsync()
    {
        if (SelectedVehicle == null) return;

        // TODO: Implémenter boîte de dialogue de modification
        // var updateDto = new UpdateVehicleDto { ... };
        // var result = await _vehicleService.UpdateVehicleAsync(SelectedVehicle.Id, updateDto);

        // if (result.IsSuccess)
        // {
        //     await LoadVehiclesAsync();
        // }
    }

    private async Task DeleteVehicleAsync()
    {
        if (SelectedVehicle == null) return;

        // TODO: Implémenter boîte de dialogue de confirmation
        // var result = await _vehicleService.DeleteVehicleAsync(SelectedVehicle.Id);

        // if (result.IsSuccess && result.Value)
        // {
        //     await LoadVehiclesAsync();
        // }
    }

    private async Task RefreshAllAsync()
    {
        await LoadVehiclesAsync();
        await LoadVehicleTypesAsync();
    }
}