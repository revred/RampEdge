# Maker.RampEdge

[![CI/CD](https://github.com/revred/RampEdge/actions/workflows/ci.yml/badge.svg)](https://github.com/revred/RampEdge/actions/workflows/ci.yml)
[![Security](https://github.com/revred/RampEdge/actions/workflows/security.yml/badge.svg)](https://github.com/revred/RampEdge/actions/workflows/security.yml)
[![NuGet](https://img.shields.io/nuget/v/Maker.RampEdge.svg)](https://www.nuget.org/packages/Maker.RampEdge/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Maker.RampEdge.svg)](https://www.nuget.org/packages/Maker.RampEdge/)

A comprehensive .NET client library for integrating Blazor WebAssembly applications with the RampEdge backend API. Provides robust authentication, automatic token management, and HTTP handlers for secure and efficient API communication.

## 🚀 Quick Start

Install the package:
```bash
dotnet add package Maker.RampEdge
```

Configure in your Blazor WebAssembly app:
```csharp
builder.Services.AddRampEdgeClient(builder.Configuration);
```

## ✨ Features

- **🔐 Authentication Management**: Complete lifecycle with automatic token refresh
- **💾 Token Storage**: Secure browser localStorage integration
- **🌐 HTTP Pipeline**: Bearer token attachment and business unit headers
- **⚙️ Easy Configuration**: Both JSON and programmatic setup options
- **🔧 Dependency Injection**: Seamless .NET DI integration
- **🧪 Fully Tested**: Comprehensive unit tests with 17+ passing tests

## 📦 Package Structure

```
Maker.RampEdge/
├── 📁 Configuration/          # Settings and options
├── 📁 Extensions/             # DI registration helpers
├── 📁 Http/                   # HTTP message handlers
├── 📁 Services/               # Core services and contracts
└── 📁 Tests/                  # Comprehensive test suite
```

## 🔧 Configuration

### JSON Configuration (appsettings.json)
```json
{
  "RampEdge": {
    "BaseAddress": "https://api.rampedge.com",
    "BusinessUnitKey": "your-business-unit-key",
    "TokenRefreshThresholdMinutes": 5,
    "EnableAutoTokenRefresh": true
  }
}
```

### Service Registration
```csharp
// Basic setup
builder.Services.AddRampEdgeClient(builder.Configuration);

// With custom configuration
builder.Services.AddRampEdgeClient(settings =>
{
    settings.BaseAddress = "https://api.rampedge.com";
    settings.BusinessUnitKey = "your-key";
});

// Advanced setup with callbacks
builder.Services.AddRampEdgeClient(
    builder.Configuration,
    onUnauthorized: async (request) => await HandleLogout());
```

## 🔗 HTTP Clients

The package automatically configures two HTTP clients:

- **`RampEdgeAuth`**: Public endpoints (login, registration)
- **`RampEdgeApi`**: Authenticated endpoints with automatic token management

```csharp
@inject IHttpClientFactory HttpClientFactory

// For authenticated calls
var apiClient = HttpClientFactory.CreateClient("RampEdgeApi");
var products = await apiClient.GetFromJsonAsync<Product[]>("/api/products");

// For public calls
var authClient = HttpClientFactory.CreateClient("RampEdgeAuth");
var info = await authClient.GetFromJsonAsync<PublicInfo>("/api/public/info");
```

## 🏗️ Architecture

### HTTP Pipeline
1. **StaticAppHeadersHandler** → Adds BusinessUnitKey header
2. **BearerTokenHandler** → Manages authentication tokens
   - Checks token expiry before requests
   - Automatically refreshes expired tokens
   - Handles 401 responses with callbacks

### Token Management
- Automatic refresh when tokens near expiry (configurable threshold)
- Browser localStorage persistence
- Secure handling with JavaScript interop
- Event-driven authentication state changes

## 🔄 Migration from Maker.Client

| Maker.Client | Maker.RampEdge |
|--------------|----------------|
| `Maker.Client.*` | `Maker.RampEdge.*` |
| `MakerAI` config section | `RampEdge` config section |
| `AddMakerClient()` | `AddRampEdgeClient()` |
| `IsMakerAIUser` | `IsRampEdgeUser` |
| `"Api"/"Auth"` clients | `"RampEdgeApi"/"RampEdgeAuth"` |

## 🧪 Testing

The package includes comprehensive tests:
- Unit tests for all core components
- HTTP handler testing with mocks
- Service registration validation
- Browser interop simulation

Run tests:
```bash
dotnet test
```

## 🚀 CI/CD & Automation

This repository includes enterprise-grade automation:

- **✅ Continuous Integration**: Automated builds and testing
- **🔒 Security Scanning**: CodeQL, dependency analysis, vulnerability detection
- **📊 Quality Gates**: SonarCloud analysis, code coverage, linting
- **📦 Automated Publishing**: NuGet.org and GitHub Packages
- **🔄 Dependency Management**: Dependabot updates
- **📋 Issue Templates**: Structured bug reports and feature requests

## 📚 Documentation

- **[Package Documentation](Maker.RampEdge/README.md)**: Detailed usage guide
- **[Developer Guide](CLAUDE.md)**: Development context and architecture
- **[CI/CD Guide](CI-CD.md)**: Complete automation documentation

## 🤝 Contributing

We welcome contributions! Please:

1. Fork the repository
2. Create a feature branch
3. Make your changes with tests
4. Submit a pull request

See our [issue templates](.github/ISSUE_TEMPLATE/) for bug reports and feature requests.

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## 🔗 Links

- **NuGet Package**: [Maker.RampEdge](https://www.nuget.org/packages/Maker.RampEdge/)
- **GitHub Repository**: [revred/RampEdge](https://github.com/revred/RampEdge)
- **Issues**: [Report bugs or request features](https://github.com/revred/RampEdge/issues)

---

Made with ❤️ for the .NET community