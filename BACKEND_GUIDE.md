# Backend Integration Guide

This guide explains how to connect the `CarRental.Desktop` application to a real backend API.

## 1. Configuration

The application uses `appsettings.json` for configuration.
Open `CarRental.Desktop/appsettings.json`:

```json
{
  "ApiBaseUrl": "https://localhost:5001/api",
  "UseMockServices": true
}
```

- Set `"UseMockServices": false` to switch to real services.
- Update `"ApiBaseUrl"` to your actual API endpoint.

## 2. Implementing Real Services

You need to implement the service interfaces found in `Services/` folder.
Recommended approach:

1. Create a `Services/Api/` folder.
2. Create classes like `ApiVehicleService.cs` implementing `IVehicleService`.
3. Use `HttpClient` to call your API.

Example `ApiVehicleService.cs`:

```csharp
using System.Net.Http;
using System.Net.Http.Json;

public class ApiVehicleService : IVehicleService
{
    private readonly HttpClient _httpClient;

    public ApiVehicleService(string baseUrl)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<List<Vehicle>> GetAllVehiclesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Vehicle>>("vehicles");
    }

    public async Task AddVehicleAsync(Vehicle vehicle)
    {
        await _httpClient.PostAsJsonAsync("vehicles", vehicle);
    }
}
```

## 3. Registering Services

Open `App.xaml.cs`.
Locate the constructor `App()`:

```csharp
            if (config.UseMockServices)
            {
                // ... Mocks ...
            }
            else
            {
                // Initialize Real Services here using config.ApiBaseUrl
                
                // Example:
                // _vehicleService = new ApiVehicleService(config.ApiBaseUrl);
                // _rentalService = new ApiRentalService(config.ApiBaseUrl);
                
                throw new NotImplementedException("Real services are not yet implemented...");
            }
```

Uncomment and update the initialization lines to use your new API service implementations.
