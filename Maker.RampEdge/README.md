# Maker.RampEdge

Maker.RampEdge is a comprehensive client library for integrating Blazor WebAssembly applications with the RampEdge backend API. It provides robust authentication, token management, and HTTP handlers to streamline secure and efficient API communication.

## Features

- **IAuthenticationService**: Complete authentication lifecycle management with automatic token refresh
- **ITokenStorage**: Secure browser-based token storage using localStorage
- **BearerTokenHandler**: Automatic Bearer token attachment with refresh logic
- **StaticAppHeadersHandler**: Business unit key header injection
- **ServiceCollectionExtensions**: Simplified dependency injection configuration
- **Dual HttpClient Configuration**: Separate clients for public and authenticated endpoints

## Installation

Add the Maker.RampEdge package to your Blazor WebAssembly project:

```bash
dotnet add package Maker.RampEdge
```

## Configuration

### 1. Configure in appsettings.json

```json
{
  "RampEdge": {
    "BaseAddress": "https://api.rampedge.com",
    "ProductBaseUrl": "https://products.rampedge.com",
    "BusinessUnitKey": "your-business-unit-key",
    "TokenRefreshThresholdMinutes": 5,
    "EnableAutoTokenRefresh": true
  }
}
```

### 2. Register Services in Program.cs

#### Using Configuration

```csharp
using Maker.RampEdge.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddRampEdgeClient(
    builder.Configuration,
    onUnauthorized: async (request) =>
    {
        // Handle unauthorized access
        await ShowLoginDialog();
    });

await builder.Build().RunAsync();
```

#### Using Direct Configuration

```csharp
builder.Services.AddRampEdgeClient(
    settings =>
    {
        settings.BaseAddress = "https://api.rampedge.com";
        settings.BusinessUnitKey = "your-business-unit-key";
        settings.TokenRefreshThresholdMinutes = 5;
        settings.EnableAutoTokenRefresh = true;
    });
```

### 3. Advanced Configuration

```csharp
builder.Services.AddRampEdgeClient(
    builder.Configuration,
    configurePublicClient: client =>
    {
        // Configure public API client
        client.AddPolicyHandler(GetRetryPolicy());
    },
    configureSecureClient: client =>
    {
        // Configure authenticated API client
        client.AddPolicyHandler(GetRetryPolicy());
        client.SetHandlerLifetime(TimeSpan.FromMinutes(5));
    },
    onUnauthorized: async (request) =>
    {
        // Navigate to login page
        Navigation.NavigateTo("/login");
    });
```

## Usage

### Authentication Service

```csharp
@inject IAuthenticationService AuthService
@inject IHttpClientFactory HttpClientFactory

@code {
    private async Task Login()
    {
        var success = await AuthService.LoginAsync("user@example.com", "password");
        if (success)
        {
            // User is authenticated
            var userName = AuthService.UserName;
            var isRampEdgeUser = AuthService.IsRampEdgeUser;
        }
    }

    private async Task Logout()
    {
        await AuthService.LogoutAsync();
    }

    private async Task CheckAuthStatus()
    {
        if (AuthService.IsAuthenticated)
        {
            var token = await AuthService.GetAccessTokenAsync();
            var expiry = await AuthService.GetAccessTokenExpiryAsync();
        }
    }
}
```

### Making API Calls

```csharp
@inject IHttpClientFactory HttpClientFactory

@code {
    private async Task<List<Product>> GetProducts()
    {
        // Use authenticated client
        var httpClient = HttpClientFactory.CreateClient("RampEdgeApi");
        var response = await httpClient.GetFromJsonAsync<List<Product>>("/api/products");
        return response;
    }

    private async Task<PublicInfo> GetPublicInfo()
    {
        // Use public client
        var httpClient = HttpClientFactory.CreateClient("RampEdgeAuth");
        var response = await httpClient.GetFromJsonAsync<PublicInfo>("/api/public/info");
        return response;
    }
}
```

### Token Storage

```csharp
@inject ITokenStorage TokenStorage

@code {
    private async Task StoreCustomData()
    {
        await TokenStorage.SetAsync("custom_key", "custom_value");
        var value = await TokenStorage.GetAsync("custom_key");
        await TokenStorage.RemoveAsync("custom_key");
    }
}
```

### Listening to Authentication Changes

```csharp
@implements IDisposable
@inject IAuthenticationService AuthService

@code {
    protected override void OnInitialized()
    {
        AuthService.OnChange += HandleAuthenticationChanged;
    }

    private void HandleAuthenticationChanged()
    {
        // React to authentication state changes
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        AuthService.OnChange -= HandleAuthenticationChanged;
    }
}
```

## Architecture

### Project Structure

```
Maker.RampEdge/
├── Configuration/
│   └── RampEdgeSettings.cs        # Configuration model
├── Extensions/
│   └── ServiceCollectionExtensions.cs  # DI registration helpers
├── Http/
│   ├── BearerTokenHandler.cs      # Bearer token attachment
│   └── StaticAppHeadersHandler.cs # Static header injection
├── Services/
│   ├── Contracts/
│   │   ├── IAuthenticationService.cs  # Auth service interface
│   │   └── ITokenStorage.cs           # Token storage interface
│   └── TokenStorage.cs                # localStorage implementation
└── README.md
```

### HTTP Pipeline

1. **StaticAppHeadersHandler**: Adds BusinessUnitKey header
2. **BearerTokenHandler**: Manages Bearer token lifecycle
   - Checks token expiry before requests
   - Automatically refreshes expired tokens
   - Handles 401 responses with callback

## Security Considerations

- Tokens are stored in browser localStorage
- Automatic token refresh prevents unnecessary re-authentication
- Configurable refresh threshold (default: 5 minutes before expiry)
- Unauthorized callbacks for custom handling
- Separate HTTP clients for public and authenticated endpoints

## Migration from Maker.Client

If migrating from the original Maker.Client package:

1. Update namespace references from `Maker.Client` to `Maker.RampEdge`
2. Change configuration section from `MakerAI` to `RampEdge`
3. Update service registration from `AddMakerClient` to `AddRampEdgeClient`
4. Replace `IsMakerAIUser` with `IsRampEdgeUser`
5. Update HTTP client names from `Api`/`Auth` to `RampEdgeApi`/`RampEdgeAuth`

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues, questions, or suggestions, please open an issue on the GitHub repository.