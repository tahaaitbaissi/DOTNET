using System;
using CarRental.Desktop.ViewModels;

namespace CarRental.Desktop.Services
{
    public interface INavigationService
    {
        ViewModelBase CurrentViewModel { get; }
        void NavigateTo<T>() where T : ViewModelBase;
        event Action<ViewModelBase?> CurrentViewModelChanged;
    }
}
