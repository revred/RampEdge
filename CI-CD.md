# CI/CD Documentation for Maker.RampEdge

This document outlines the complete CI/CD infrastructure for the Maker.RampEdge NuGet package.

## Overview

The CI/CD pipeline automates:
- **Build and Test**: Automated testing on every push and PR
- **Security Scanning**: Vulnerability detection and dependency analysis
- **Code Quality**: SonarCloud analysis and linting
- **Package Publishing**: Automated NuGet.org and GitHub Packages publishing
- **Release Management**: Automated GitHub releases with changelog generation

## Workflows

### 1. CI/CD Pipeline (`ci.yml`)

**Triggers:**
- Push to `master`, `main`, `develop` branches
- Pull requests to `master`, `main`
- Release publications

**Jobs:**
- `build-and-test`: Builds, tests, and creates NuGet packages
- `publish-nuget`: Publishes to NuGet.org on releases
- `publish-github-packages`: Publishes to GitHub Packages on master push

**Features:**
- .NET 9.0 support
- Dependency caching for faster builds
- Code coverage reporting with Codecov
- Artifact storage for packages

### 2. Release Management (`release.yml`)

**Triggers:**
- Tags matching `v*.*.*` pattern
- Manual workflow dispatch with version input

**Features:**
- Automatic version detection from tags
- Release notes generation from git history
- GitHub release creation with package attachments
- Automatic NuGet.org publishing for stable releases
- Pre-release support

### 3. Security Scanning (`security.yml`)

**Triggers:**
- Push to main branches
- Pull requests
- Weekly scheduled scans (Mondays 2 AM UTC)

**Security Features:**
- CodeQL analysis for C# code
- .NET security audit for vulnerable packages
- Dependency review for PRs
- Package validation using Microsoft tools
- Trivy vulnerability scanning
- SARIF result upload to GitHub Security tab

### 4. Code Quality (`quality.yml`)

**Triggers:**
- Push to main branches and develop
- Pull requests

**Quality Checks:**
- SonarCloud analysis with coverage metrics
- Code formatting verification
- Build warnings treated as errors
- Documentation completeness checks
- Markdown linting
- Performance testing
- Package size analysis

## Required Secrets

Set these secrets in your GitHub repository settings:

### NuGet Publishing
- `NUGET_API_KEY`: Your NuGet.org API key for package publishing

### Code Quality (Optional)
- `SONAR_TOKEN`: SonarCloud token for code quality analysis

### Notes
- `GITHUB_TOKEN` is automatically provided by GitHub Actions

## Getting Your NuGet API Key

1. Go to [NuGet.org](https://www.nuget.org/)
2. Sign in with your account
3. Go to Account Settings → API Keys
4. Create a new API key with:
   - **Key Name**: `GitHub Actions - RampEdge`
   - **Select Scopes**: `Push new packages and package versions`
   - **Select Packages**: `*` (or specific to your package)
   - **Glob Pattern**: `Maker.RampEdge*`

## Release Process

### Automatic Release (Recommended)

1. **Create and Push a Tag:**
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

2. **The workflow will:**
   - Build and test the package
   - Create a GitHub release
   - Generate release notes
   - Publish to NuGet.org (if not pre-release)

### Manual Release

1. **Go to GitHub Actions**
2. **Select "Release Management" workflow**
3. **Click "Run workflow"**
4. **Enter version and pre-release flag**

## Package Versioning

The package version is automatically managed:
- **Development builds**: Use the version in `.csproj`
- **Release builds**: Use the tag version (e.g., `v1.2.3` → `1.2.3`)

## Branch Protection

Recommended branch protection rules for `master`:

1. **Go to Settings → Branches**
2. **Add rule for `master`:**
   - Require PR reviews (1+ reviewers)
   - Require status checks to pass:
     - `build-and-test`
     - `security-scan`
     - `code-quality`
   - Require branches to be up to date
   - Include administrators

## Monitoring and Notifications

### GitHub Notifications
- Failed builds notify repository maintainers
- Security alerts are posted to the Security tab
- Dependabot creates PRs for dependency updates

### External Integration
- **Codecov**: Code coverage reporting
- **SonarCloud**: Code quality metrics
- **NuGet.org**: Package download statistics

## Dependabot Configuration

Automatic dependency updates for:
- **NuGet packages**: Weekly updates on Mondays
- **GitHub Actions**: Weekly updates on Mondays
- **Grouped updates** for related packages (Microsoft.Extensions.*, testing packages)

## Troubleshooting

### Common Issues

1. **NuGet Publish Fails**
   - Check `NUGET_API_KEY` secret is set correctly
   - Verify API key has correct permissions
   - Check if package version already exists

2. **Tests Fail in CI**
   - Ensure all tests pass locally
   - Check for environment-specific issues
   - Review test output in Actions logs

3. **Security Scan Alerts**
   - Review alerts in Security tab
   - Update vulnerable dependencies
   - Consider suppressions for false positives

4. **SonarCloud Analysis Fails**
   - Verify `SONAR_TOKEN` secret
   - Check SonarCloud project configuration
   - Review coverage report paths

### Build Badges

Add these badges to your README:

```markdown
[![CI/CD](https://github.com/revred/RampEdge/actions/workflows/ci.yml/badge.svg)](https://github.com/revred/RampEdge/actions/workflows/ci.yml)
[![Security](https://github.com/revred/RampEdge/actions/workflows/security.yml/badge.svg)](https://github.com/revred/RampEdge/actions/workflows/security.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=revred_RampEdge&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=revred_RampEdge)
[![NuGet](https://img.shields.io/nuget/v/Maker.RampEdge.svg)](https://www.nuget.org/packages/Maker.RampEdge/)
```

## Performance Optimization

### Build Performance
- Dependency caching reduces build times
- Parallel job execution where possible
- Conditional job execution (e.g., publish only on releases)

### Resource Usage
- Uses `ubuntu-latest` for cost efficiency
- Minimal tool installations
- Shared artifacts between jobs

## Security Best Practices

- **Least Privilege**: API keys have minimal required permissions
- **Secret Rotation**: Regularly rotate API keys and tokens
- **Dependency Scanning**: Automated vulnerability detection
- **Code Analysis**: Static security analysis with CodeQL
- **Branch Protection**: Prevents direct pushes to main branches

## Future Enhancements

Potential improvements:
- **Multi-platform testing** (Windows, macOS, Linux)
- **Integration testing** with real API endpoints
- **Performance benchmarking** over time
- **Automatic changelog generation** from conventional commits
- **Slack/Teams notifications** for releases and failures