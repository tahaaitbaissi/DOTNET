using CarRental.Application.DTOs;
using CarRental.Desktop.Services;
using CarRental.Desktop.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService; 

        private string _email = string.Empty;
        private string _password = string.Empty;

        public LoginViewModel(IDialogService dialogService, INavigationService navigationService)
        {
            _dialogService = dialogService;
            _navigationService = navigationService;

            LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
        }

        private async Task LoginAsync()
        {
            try
            {
                IsLoading = true;
                ClearError();

                await Task.Delay(500);

                if (Email == "admin@test.com" && Password == "admin")
                {
                    SessionManager.Login(Email, "Admin");

                    _navigationService.NavigateTo<DashboardViewModel>();
                }
                else
                {
                    ErrorMessage = "Email ou mot de passe incorrect";
                    await _dialogService.ShowMessageAsync("Erreur", ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await _dialogService.ShowMessageAsync("Erreur", ErrorMessage);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}