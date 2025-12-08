using CarRental.Desktop.ViewModels.Base;
using System;

namespace CarRental.Desktop.Services;

public class NavigationService : INavigationService
{
    private ViewModelBase? _currentViewModel;

    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            _currentViewModel = value;
            CurrentViewModelChanged?.Invoke(value);
        }
    }

    public event Action<ViewModelBase?>? CurrentViewModelChanged;

    public void NavigateTo<T>() where T : ViewModelBase
    {
        // Création simple - à améliorer avec DI
        var viewModel = Activator.CreateInstance<T>();
        CurrentViewModel = viewModel;
    }

    public void NavigateTo(ViewModelBase viewModel)
    {
        CurrentViewModel = viewModel;
    }

    public void GoBack()
    {
        // Implémentation basique
        CurrentViewModel = null;
    }
}