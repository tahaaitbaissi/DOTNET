using CarRental.Desktop.ViewModels.Base;
using System;

namespace CarRental.Desktop.Services;

public interface INavigationService
{
    ViewModelBase? CurrentViewModel { get; }
    void NavigateTo<T>() where T : ViewModelBase;
    void NavigateTo(ViewModelBase viewModel);
    void GoBack();
    event Action<ViewModelBase?> CurrentViewModelChanged;
}