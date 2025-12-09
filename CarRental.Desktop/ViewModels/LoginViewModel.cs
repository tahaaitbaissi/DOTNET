using System.Windows.Input;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthenticationService _authService;

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _errorMessage = string.Empty;
        public new string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(INavigationService navigationService, IAuthenticationService authService)
        {
            _navigationService = navigationService;
            _authService = authService;
            LoginCommand = new RelayCommand(Login);
        }

        private async void Login(object? obj)
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter email and password.";
                return;
            }

            var success = await _authService.LoginAsync(Email, Password);

            if (success)
            {
                _navigationService.NavigateTo<DashboardViewModel>();
            }
            else
            {
                ErrorMessage = "Invalid credentials. Please try again.";
            }
        }
    }
}
