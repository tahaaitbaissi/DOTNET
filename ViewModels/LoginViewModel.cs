using System.Windows.Input;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public ICommand LoginCommand { get; }

        public LoginViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            LoginCommand = new RelayCommand(Login);
        }

        private void Login(object? obj)
        {
            // TODO: Implement actual authentication logic
            _navigationService.NavigateTo<DashboardViewModel>();
        }
    }
}
