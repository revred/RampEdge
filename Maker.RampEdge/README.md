# Maker.RampEdge.Blazor

Maker.RampEdge.Blazor is a client library designed to facilitate communication between Blazor WebAssembly applications and the Maker backend API. It provides authentication, token management, and HTTP handlers to streamline secure and efficient API calls from your Blazor app.

## Features

- **AuthenticationService**: Handles user authentication and token retrieval.
- **TokenStorage**: Manages storage and retrieval of authentication tokens.
- **BearerTokenHandler**: Automatically attaches bearer tokens to outgoing HTTP requests.
- **StaticAppHeadersHandler**: Adds static headers required by the Maker backend.
- **ServiceCollectionExtensions**: Extension methods for easy DI registration.

## Getting Started

### 1. Install the Package

Add a reference to `Maker.RampEdge.Blazor` in your Blazor WebAssembly project:

```bash
dotnet add package Maker.RampEdge.Blazor
```

### 2. Register Services

In your `Program.cs` or `Startup.cs`, register the Maker client services:

```csharp
builder.Services.AddMakerClient(options =>
{
    options.BaseUrl = "https://api.maker.example.com";
    // Configure other options as needed
});
```

### 3. Use the Authentication Service

Inject and use the authentication service in your components:

```csharp
@inject IAuthenticationService AuthService

// Example usage
var result = await AuthService.LoginAsync(username, password);
if (result.Success)
{
    // User is authenticated
}
```

### 4. Make API Calls

Authenticated HTTP requests will automatically include the bearer token:

```csharp
var httpClient = HttpClientFactory.CreateClient("MakerApi");
var response = await httpClient.GetAsync("/endpoint");
```

## Folder Structure

- `Services/` - Core service implementations and contracts
- `Http/` - HTTP handlers for authentication and headers
- `Extensions/` - DI registration helpers
- `Configuration/` - Settings and configuration classes

## Contributing

Feel free to open issues or submit pull requests for improvements or bug fixes.

## License

This project is licensed under the MIT License.
