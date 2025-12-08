using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CarRental.Desktop.ViewModels;

namespace CarRental.Desktop.Services
{
    public interface INavigationService
    {
        ViewModelBase CurrentView { get; }
        void NavigateTo<T>() where T : ViewModelBase;
        event Action CurrentViewChanged;
    }

    public class NavigationService : INavigationService
    {
        private ViewModelBase _currentView;
        private readonly Func<Type, ViewModelBase> _viewModelFactory;

        public ViewModelBase CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                CurrentViewChanged?.Invoke();
            }
        }

        public event Action CurrentViewChanged;

        public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<T>() where T : ViewModelBase
        {
            CurrentView = _viewModelFactory(typeof(T));
        }
    }
}
