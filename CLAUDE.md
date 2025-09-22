# CLAUDE.md - Maker.RampEdge Project Context

## Project Overview

Maker.RampEdge is a NuGet package that provides a comprehensive client library for Blazor WebAssembly applications to communicate with the RampEdge backend API. This project is derived from the Maker.Client implementation in the HippoHex project but has been rebranded and optimized for the RampEdge ecosystem.

## Project Structure

```
D:\Code\RampEdge\
├── Maker.RampEdge.sln              # Solution file
└── Maker.RampEdge/
    ├── Maker.RampEdge.csproj       # Project file with NuGet package configuration
    ├── Configuration/
    │   └── RampEdgeSettings.cs     # Settings model for API configuration
    ├── Extensions/
    │   └── ServiceCollectionExtensions.cs  # DI registration extensions
    ├── Http/
    │   ├── BearerTokenHandler.cs   # Automatic Bearer token management
    │   └── StaticAppHeadersHandler.cs  # Business unit key header injection
    ├── Services/
    │   ├── Contracts/
    │   │   ├── IAuthenticationService.cs  # Auth service interface
    │   │   └── ITokenStorage.cs    # Token storage interface
    │   └── TokenStorage.cs         # Browser localStorage implementation
    └── README.md                    # User-facing documentation
```

## Key Components

### Authentication System
- **IAuthenticationService**: Core authentication interface (implementation expected to be provided by consuming applications)
- **TokenStorage**: Browser-based token storage using JavaScript interop
- **BearerTokenHandler**: Automatic token refresh and attachment to HTTP requests

### HTTP Pipeline
- **StaticAppHeadersHandler**: Adds required headers like BusinessUnitKey
- **BearerTokenHandler**: Manages authentication tokens with automatic refresh
- Two separate HTTP clients: `RampEdgeAuth` (public) and `RampEdgeApi` (authenticated)

### Configuration
- **RampEdgeSettings**: Configuration model for API endpoints and authentication settings
- Support for both appsettings.json and programmatic configuration

## Development Guidelines

### Code Standards
- Target Framework: .NET 9.0
- Language: C# with nullable reference types enabled
- Naming Convention: PascalCase for public members, camelCase for private fields with underscore prefix

### Package Management
- Version: Currently 1.0.0
- Dependencies:
  - Microsoft.AspNetCore.Components.Web 9.0.8
  - Microsoft.Extensions.* 9.0.8
  - System.IdentityModel.Tokens.Jwt 8.14.0

### Testing Approach
- Unit tests should be added in the Tests folder
- Integration tests should mock JavaScript interop for TokenStorage
- HTTP handlers should be tested with HttpMessageHandler mocks

## Common Tasks

### Building the Package
```bash
dotnet build -c Release
dotnet pack -c Release
```

### Publishing to NuGet
```bash
dotnet nuget push bin/Release/Maker.RampEdge.1.0.0.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY
```

### Local Testing
1. Build the package: `dotnet pack`
2. Create a local NuGet source: `dotnet nuget add source D:\Code\RampEdge\Maker.RampEdge\bin\Debug --name RampEdgeLocal`
3. Reference in test project: `dotnet add package Maker.RampEdge --version 1.0.0 --source RampEdgeLocal`

## Architecture Decisions

### Why Separate HTTP Clients?
- **RampEdgeAuth**: For public endpoints (login, registration, public info)
- **RampEdgeApi**: For authenticated endpoints with automatic token management

### Token Management Strategy
- Tokens stored in browser localStorage for persistence
- Automatic refresh when token is within 5 minutes of expiry
- Unauthorized callbacks allow applications to handle auth failures gracefully

### Extensibility Points
- HTTP client builders can be customized during registration
- Authentication service implementation is provided by consuming application
- Unauthorized callback allows custom handling of 401 responses

## Migration from Maker.Client

Key changes when migrating from the original Maker.Client:

1. **Namespace**: `Maker.Client` → `Maker.RampEdge`
2. **Configuration Section**: `MakerAI` → `RampEdge`
3. **Service Registration**: `AddMakerClient` → `AddRampEdgeClient`
4. **Property Names**: `IsMakerAIUser` → `IsRampEdgeUser`
5. **HTTP Client Names**: `Api`/`Auth` → `RampEdgeApi`/`RampEdgeAuth`

## Future Enhancements

Potential improvements for future versions:

1. **Caching**: Add response caching for frequently accessed endpoints
2. **Retry Policies**: Built-in Polly retry policies for transient failures
3. **Telemetry**: OpenTelemetry integration for observability
4. **Offline Support**: Queue requests when offline and sync when connected
5. **Token Encryption**: Encrypt tokens in localStorage for additional security
6. **Multi-tenant Support**: Enhanced business unit key management

## Troubleshooting

### Common Issues

1. **Missing Configuration**: Ensure `RampEdge` section exists in appsettings.json
2. **CORS Errors**: Backend must allow the Blazor app's origin
3. **Token Expiry**: Check `TokenRefreshThresholdMinutes` setting
4. **JavaScript Interop**: Ensure Blazor WebAssembly is properly initialized

### Debug Logging

Enable debug logging in development:
```csharp
#if DEBUG
services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Debug));
#endif
```

## Contact and Support

For issues specific to this package:
- Create an issue in the project repository
- Include version number, error messages, and reproduction steps

## Version History

### v1.0.0 (Current)
- Initial release based on Maker.Client architecture
- Core authentication and HTTP handling features
- Support for .NET 9.0 and Blazor WebAssembly