# DescopeSampleApp - .NET 8 Migration

This application has been migrated from ASP.NET Web Forms 4.8 to .NET 8 using clean architecture principles.

## Architecture

The solution follows clean architecture with four main layers:

### Domain Layer (`DescopeSampleApp.Domain`)
- Contains domain entities and interfaces
- No dependencies on other projects
- Core business logic and rules

### Application Layer (`DescopeSampleApp.Application`)
- Contains service implementations
- References Domain layer only
- Business logic orchestration

### Infrastructure Layer (`DescopeSampleApp.Infrastructure`)
- Contains data access implementations
- Entity Framework Core DbContext and repositories
- References Domain and Application layers

### Web Layer (`DescopeSampleApp.Web`)
- ASP.NET Core Razor Pages application
- Controllers for API endpoints
- References Infrastructure and Application layers

## Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or VS Code

## Getting Started

1. Clone the repository
2. Navigate to the solution directory
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Build the solution:
   ```bash
   dotnet build
   ```
5. Run the application:
   ```bash
   dotnet run --project src/DescopeSampleApp.Web
   ```

## Configuration

The application uses `appsettings.json` for configuration. Key settings:

- **Descope:ProjectId**: Your Descope project ID (default: P2dI0leWLEC45BDmfxeOCSSOWiCt)
- **Logging**: Serilog configuration

## Migration Notes

### What Was Migrated

1. **Web Forms to Razor Pages**
   - `Login.aspx` → `Pages/Login.cshtml`
   - `AuthenticatedPage.aspx` → `Pages/AuthenticatedPage.cshtml`

2. **Controllers**
   - `HomeController` migrated to ASP.NET Core MVC
   - `SampleController` migrated to ASP.NET Core Web API

3. **Authentication**
   - Descope JWT token validation implemented using ASP.NET Core authentication middleware
   - Token validation service with proper dependency injection

4. **Configuration**
   - `Web.config` → `appsettings.json`
   - Environment-specific settings in `appsettings.Development.json`

5. **Dependency Injection**
   - All services registered in `Program.cs`
   - Constructor injection throughout the application

### Key Differences from Web Forms

1. **No ViewState**: State management uses modern patterns (TempData, sessions, client-side storage)
2. **No Page Lifecycle Events**: Razor Pages use simpler OnGet/OnPost handlers
3. **No Global.asax**: Application startup configured in `Program.cs`
4. **No Server Controls**: HTML helpers and tag helpers replace server controls
5. **Async/Await**: All I/O operations are async

## Running Tests

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test tests/DescopeSampleApp.UnitTests

# Run integration tests only
dotnet test tests/DescopeSampleApp.IntegrationTests
```

## Project Structure

```
Component/
├── src/
│   ├── DescopeSampleApp.Domain/
│   ├── DescopeSampleApp.Application/
│   ├── DescopeSampleApp.Infrastructure/
│   └── DescopeSampleApp.Web/
├── tests/
│   ├── DescopeSampleApp.UnitTests/
│   └── DescopeSampleApp.IntegrationTests/
└── DescopeSampleApp.sln
```

## API Endpoints

- `GET /api/sample` - Sample authenticated endpoint (requires Bearer token)

## Pages

- `/` - Home page
- `/Login` - Login page with Descope authentication
- `/AuthenticatedPage` - Protected page (requires authentication)

## Known Issues

None at this time.

## Future Improvements

1. Add database migrations for production database
2. Implement comprehensive authentication middleware
3. Add more comprehensive test coverage
4. Implement caching strategies
5. Add API documentation with Swagger/OpenAPI

## License

Copyright © 2026 DescopeSampleApp
