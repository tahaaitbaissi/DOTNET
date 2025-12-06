using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Desktop.ViewModels.Base;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;

    private string _email = string.Empty;
    private string _password = string.Empty;
    private bool _rememberMe;
    private bool _isLoggingIn;

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

    public bool RememberMe
    {
        get => _rememberMe;
        set => SetProperty(ref _rememberMe, value);
    }

    public bool IsLoggingIn
    {
        get => _isLoggingIn;
        set => SetProperty(ref _isLoggingIn, value);
    }

    public ICommand LoginCommand { get; }
    public ICommand ForgotPasswordCommand { get; }

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;

        LoginCommand = new RelayCommand(async (param) => await LoginAsync(),
            (param) => !IsLoggingIn && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password));

        ForgotPasswordCommand = new RelayCommand(async (param) => await ForgotPasswordAsync());
    }

    private async Task LoginAsync()
    {
        IsLoggingIn = true;

        try
        {
            var loginDto = new LoginDto(Email, Password);
            var result = await _authService.LoginAsync(loginDto);

            if (result.IsSuccess && result.Value != null)
            {
                // TODO: Stocker le token
                // AppState.CurrentUser = result.Value.User;
                // AppState.Token = result.Value.Token;

                // TODO: Naviguer vers le dashboard
                Console.WriteLine("Connexion réussie !");
            }
            else
            {
                Console.WriteLine($"Échec connexion: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
        finally
        {
            IsLoggingIn = false;
        }
    }

    private async Task ForgotPasswordAsync()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            // TODO: Afficher message
            return;
        }

        var result = await _authService.ForgotPasswordAsync(Email);

        if (result.IsSuccess && result.Value)
        {
            Console.WriteLine("Email de réinitialisation envoyé");
        }
    }
}