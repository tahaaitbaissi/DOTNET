using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CarRental.Desktop.ViewModels;

namespace CarRental.Desktop.Services
{


    public class NavigationService : INavigationService
    {
        private ViewModelBase _currentView;
        private readonly Func<Type, ViewModelBase> _viewModelFactory;

        public ViewModelBase CurrentViewModel
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                CurrentViewModelChanged?.Invoke(_currentView);
            }
        }

        public event Action<ViewModelBase?> CurrentViewModelChanged;

        public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<T>() where T : ViewModelBase
        {
            CurrentViewModel = _viewModelFactory(typeof(T));
        }
    }
}
