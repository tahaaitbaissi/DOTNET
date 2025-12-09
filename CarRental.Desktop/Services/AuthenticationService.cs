using System.Threading.Tasks;
using CarRental.Application.DTOs;

namespace CarRental.Desktop.Services
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(string username, string password);
        void Logout();
        UserInfoDto? CurrentUser { get; }
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApiClient _apiClient;

        public UserInfoDto? CurrentUser { get; private set; }

        public AuthenticationService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var loginDto = new LoginDto(email, password);

            var (authResponse, error) = await _apiClient.PostAsync<LoginDto, AuthResponseDto>("api/Auth/Login", loginDto);

            if (error == null && authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
            {
                _apiClient.SetToken(authResponse.Token);
                CurrentUser = authResponse.User;

                // Update local session state so UI can react to role / username
                var username = authResponse.User.FullName ?? authResponse.User.Username;
                var role = authResponse.User.Role ?? "User";
                SessionManager.Login(username, role);

                return true;
            }

            return false;
        }

        public void Logout()
        {
            _apiClient.ClearToken();
            CurrentUser = null;

            SessionManager.Logout();
        }
    }
}
