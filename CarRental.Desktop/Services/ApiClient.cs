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
            FileLogger.Log($"GET Request: {endpoint}");
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                FileLogger.Log($"GET Response {endpoint}: {response.StatusCode}");
                
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
                        FileLogger.LogError(err, ex);
                        await _dialogService?.ShowErrorAsync("Deserialization Error", err);
                        return (default, err);
                    }
                }

                // Show server-provided error
                FileLogger.LogError($"GET {endpoint} Failed: {content}");
                return (default, content);
            }
            catch (HttpRequestException hre)
            {
                FileLogger.LogError($"GET {endpoint} Network Error", hre);
                await HandleHttpExceptionAsync(hre);
                return (default, hre.Message);
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"GET {endpoint} Exception", ex);
                await _dialogService?.ShowErrorAsync("Error", ex.Message);
                return (default, ex.Message);
            }
        }

        public async Task<bool> PostAsync<T>(string endpoint, T data)
        {
            FileLogger.Log($"POST Request: {endpoint}");
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);
                FileLogger.Log($"POST Response {endpoint}: {response.StatusCode}");
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    FileLogger.LogError($"POST {endpoint} Failed: {content}");
                    await _dialogService?.ShowErrorAsync("API Error", content);
                    return false;
                }
                return true;
            }
            catch (HttpRequestException hre)
            {
                FileLogger.LogError($"POST {endpoint} Network Error", hre);
                await HandleHttpExceptionAsync(hre);
                return false;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"POST {endpoint} Exception", ex);
                await _dialogService?.ShowErrorAsync("Error", ex.Message);
                return false;
            }
        }

        public async Task<(TResult? Result, string? Error)> PostAsync<TInput, TResult>(string endpoint, TInput data)
        {
            FileLogger.Log($"POST Request (Expected Result): {endpoint}");
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);
                FileLogger.Log($"POST Response {endpoint}: {response.StatusCode}");
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
                        FileLogger.LogError(err, ex);
                        await _dialogService?.ShowErrorAsync("Deserialization Error", err);
                        return (default, err);
                    }
                }
                
                FileLogger.LogError($"POST {endpoint} Failed: {content}");
                return (default, content);
            }
            catch (HttpRequestException hre)
            {
                FileLogger.LogError($"POST {endpoint} Network Error", hre);
                await HandleHttpExceptionAsync(hre);
                return (default, hre.Message);
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"POST {endpoint} Exception", ex);
                await _dialogService?.ShowErrorAsync("Error", ex.Message);
                return (default, ex.Message);
            }
        }

        public async Task<bool> PutAsync<T>(string endpoint, T data)
        {
            FileLogger.Log($"PUT Request: {endpoint}");
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, data);
                FileLogger.Log($"PUT Response {endpoint}: {response.StatusCode}");
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    FileLogger.LogError($"PUT {endpoint} Failed: {content}");
                    await _dialogService?.ShowErrorAsync("API Error", content);
                    return false;
                }
                return true;
            }
            catch (HttpRequestException hre)
            {
                FileLogger.LogError($"PUT {endpoint} Network Error", hre);
                await HandleHttpExceptionAsync(hre);
                return false;
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"PUT {endpoint} Exception", ex);
                await _dialogService?.ShowErrorAsync("Error", ex.Message);
                return false;
            }
        }

        public async Task<(bool Success, string? Error)> PutRawAsync<T>(string endpoint, T data)
        {
            FileLogger.Log($"PUT Raw Request: {endpoint}");
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, data);
                var content = await response.Content.ReadAsStringAsync();
                FileLogger.Log($"PUT Raw Response {endpoint}: {response.StatusCode}");
                if (!response.IsSuccessStatusCode)
                {
                    FileLogger.LogError($"PUT Raw {endpoint} Failed: {content}");
                    return (false, content);
                }
                return (true, null);
            }
            catch (HttpRequestException hre)
            {
                FileLogger.LogError($"PUT Raw {endpoint} Network Error", hre);
                await HandleHttpExceptionAsync(hre);
                return (false, hre.Message);
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"PUT Raw {endpoint} Exception", ex);
                await _dialogService?.ShowErrorAsync("Error", ex.Message);
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(string endpoint)
        {
            FileLogger.Log($"DELETE Request: {endpoint}");
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();
                FileLogger.Log($"DELETE Response {endpoint}: {response.StatusCode}");
                if (!response.IsSuccessStatusCode)
                {
                    FileLogger.LogError($"DELETE {endpoint} Failed: {content}");
                    return (false, content);
                }
                return (true, null);
            }
            catch (HttpRequestException hre)
            {
                FileLogger.LogError($"DELETE {endpoint} Network Error", hre);
                await HandleHttpExceptionAsync(hre);
                return (false, hre.Message);
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"DELETE {endpoint} Exception", ex);
                await _dialogService?.ShowErrorAsync("Error", ex.Message);
                return (false, ex.Message);
            }
        }
    }
}
