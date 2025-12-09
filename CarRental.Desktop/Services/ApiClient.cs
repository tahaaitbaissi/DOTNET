using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CarRental.Desktop.Services
{
    public interface IApiClient
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<(TResult? Result, string? Error)> GetRawAsync<TResult>(string endpoint);
        Task<bool> PostAsync<T>(string endpoint, T data);
        Task<(TResult? Result, string? Error)> PostAsync<TInput, TResult>(string endpoint, TInput data);
        Task<bool> PutAsync<T>(string endpoint, T data);
        Task<(bool Success, string? Error)> PutRawAsync<T>(string endpoint, T data);
        Task<(bool Success, string? Error)> DeleteAsync(string endpoint);
        void SetToken(string token);
        void ClearToken();
    }

    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IDialogService? _dialogService;
        private string _token = string.Empty;

        public ApiClient(string baseUrl, IDialogService? dialogService = null)
        {
            // Ensure baseUrl ends with a slash so relative endpoints combine correctly
            if (string.IsNullOrWhiteSpace(baseUrl)) baseUrl = "http://localhost:5120/";
            if (!baseUrl.EndsWith('/')) baseUrl += '/';

            _dialogService = dialogService;
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetToken(string token)
        {
            _token = token;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearToken()
        {
            _token = string.Empty;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private async Task HandleHttpExceptionAsync(Exception ex)
        {
            var msg = ex.Message;
            await _dialogService?.ShowErrorAsync("Network Error", $"Failed to reach API: {msg}");
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var (result, error) = await GetRawAsync<T>(endpoint);
            if (error != null)
            {
                await _dialogService?.ShowErrorAsync("API Error", error);
            }
            return result;
        }

        public async Task<(TResult? Result, string? Error)> GetRawAsync<TResult>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var deserialized = System.Text.Json.JsonSerializer.Deserialize<TResult>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        return (deserialized, null);
                    }
                    catch (Exception ex)
                    {
                        var err = "Deserialization failed: " + ex.Message;
                        await _dialogService?.ShowErrorAsync("Deserialization Error", err);
                        return (default, err);
                    }
                }

                // Show server-provided error
                return (default, content);
            }
            catch (HttpRequestException hre)
            {
                await HandleHttpExceptionAsync(hre);
                return (default, hre.Message);
            }
            catch (Exception ex)
            {
                await _dialogService?.ShowErrorAsync("Error", ex.Message);
                return (default, ex.Message);
            }
        }

        public async Task<bool> PostAsync<T>(string endpoint, T data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    await _dialogService?.ShowErrorAsync("API Error", content);
                    return false;
                }
                return true;
            }
            catch (HttpRequestException hre)
            {
                await HandleHttpExceptionAsync(hre);
                return false;
            }
            catch (Exception ex)
            {
                await _dialogService?.ShowErrorAsync("Error", ex.Message);
                return false;
            }
        }

        public async Task<(TResult? Result, string? Error)> PostAsync<TInput, TResult>(string endpoint, TInput data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var deserialized = System.Text.Json.JsonSerializer.Deserialize<TResult>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        return (deserialized, null);
                    }
                    catch (Exception ex)
                    {
                        var err = "Deserialization failed: " + ex.Message;
                        await _dialogService?.ShowErrorAsync("Deserialization Error", err);
                        return (default, err);
                    }
                }

                return (default, content);
            }
            catch (HttpRequestException hre)
            {
                await HandleHttpExceptionAsync(hre);
                return (default, hre.Message);
            }
            catch (Exception ex)
            {
                await _dialogService?.ShowErrorAsync("Error", ex.Message);
                return (default, ex.Message);
            }
        }

        public async Task<bool> PutAsync<T>(string endpoint, T data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, data);
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    await _dialogService?.ShowErrorAsync("API Error", content);
                    return false;
                }
                return true;
            }
            catch (HttpRequestException hre)
            {
                await HandleHttpExceptionAsync(hre);
                return false;
            }
            catch (Exception ex)
            {
                await _dialogService?.ShowErrorAsync("Error", ex.Message);
                return false;
            }
        }

        public async Task<(bool Success, string? Error)> PutRawAsync<T>(string endpoint, T data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, data);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return (false, content);
                }
                return (true, null);
            }
            catch (HttpRequestException hre)
            {
                await HandleHttpExceptionAsync(hre);
                return (false, hre.Message);
            }
            catch (Exception ex)
            {
                await _dialogService?.ShowErrorAsync("Error", ex.Message);
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return (false, content);
                }
                return (true, null);
            }
            catch (HttpRequestException hre)
            {
                await HandleHttpExceptionAsync(hre);
                return (false, hre.Message);
            }
            catch (Exception ex)
            {
                await _dialogService?.ShowErrorAsync("Error", ex.Message);
                return (false, ex.Message);
            }
        }
    }
}
