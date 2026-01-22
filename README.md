# DescopeSampleApp - .NET 8 Migration

This project has been successfully migrated from ASP.NET Web Forms 4.8 to .NET 8 with clean architecture.

## Project Structure

The solution follows clean architecture principles with the following layers:

### Domain Layer (`DescopeSampleApp.Domain`)
- Contains domain entities, interfaces, and domain logic
- No dependencies on other layers

### Application Layer (`DescopeSampleApp.Application`)
- Contains business logic, services, DTOs, and validators
- Depends only on Domain layer
- Uses AutoMapper for object mapping

### Infrastructure Layer (`DescopeSampleApp.Infrastructure`)
- Contains data access implementation (EF Core)
- Repository implementations
- External service integrations
- Depends on Domain and Application layers

### Web Layer (`DescopeSampleApp.Web`)
- ASP.NET Core 8 Razor Pages application
- Controllers for API endpoints
- Depends on Infrastructure and Application layers

### Test Projects
- `DescopeSampleApp.UnitTests` - Unit tests for business logic
- `DescopeSampleApp.IntegrationTests` - Integration tests

## Migration Summary

### What Was Migrated

1. **Web Forms Pages** → **Razor Pages**
   - `Login.aspx` → `Pages/Login.cshtml`
   - `AuthenticatedPage.aspx` → `Pages/AuthenticatedPage.cshtml`

2. **Global.asax** → **Program.cs**
   - Application startup logic moved to `Program.cs`
   - Middleware pipeline configured

3. **Web.config** → **appsettings.json**
   - Configuration migrated to JSON format
   - Descope Project ID configured

4. **Controllers**
   - `HomeController` migrated to ASP.NET Core MVC
   - `SampleController` (API) migrated to ASP.NET Core Web API

5. **Token Validation**
   - `TokenValidator` class migrated to use ASP.NET Core DI
   - Added proper logging and error handling

## Technologies Used

- **.NET 8**: Target framework
- **ASP.NET Core 8**: Web framework (Razor Pages + MVC)
- **Entity Framework Core 8**: ORM for data access
- **Serilog**: Structured logging
- **AutoMapper**: Object-to-object mapping
- **xUnit**: Testing framework
- **FluentAssertions**: Assertion library for tests
- **Descope**: Authentication provider (jose-jwt)

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)

### Configuration

Update `appsettings.json` with your configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your-Connection-String"
  },
  "Descope": {
    "ProjectId": "Your-Descope-Project-ID"
  }
}
```

### Running the Application

```bash
# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run the web application
dotnet run --project src/DescopeSampleApp.Web

# Run tests
dotnet test
```

The application will start at `https://localhost:5001` (or the port shown in console).

## Key Features

1. **Clean Architecture**: Separation of concerns with distinct layers
2. **Dependency Injection**: Built-in .NET DI container
3. **Structured Logging**: Serilog for comprehensive logging
4. **Token-based Authentication**: JWT validation via Descope
5. **Async/Await**: All I/O operations are asynchronous
6. **Error Handling**: Comprehensive error handling and logging
7. **Testing**: Unit and integration test projects

## API Endpoints

- `GET /api/sample` - Sample authenticated endpoint (requires Bearer token)

## Pages

- `/` - Home page
- `/Login` - Login page with Descope web component
- `/AuthenticatedPage` - Protected page requiring authentication
- `/Error` - Error page

## Migration Notes

### Breaking Changes from Web Forms

1. **ViewState**: No longer available. Use TempData or session for state management
2. **Server Controls**: Replaced with HTML helpers and Tag Helpers
3. **Page Lifecycle**: Different lifecycle in Razor Pages (OnGet, OnPost, etc.)
4. **Code-Behind**: Logic moved to service layer and page models

### Configuration Changes

- `Web.config` → `appsettings.json`
- Connection strings in JSON format
- No more `<appSettings>` section

### Known Issues

None. The build completes successfully with 0 errors and 0 warnings.

## Build Verification

✅ **Build Status**: SUCCESS

- All projects compile without errors
- All dependencies resolved correctly
- Target framework: .NET 8
- Build time: ~6 seconds

## Future Improvements

1. Add more comprehensive unit tests
2. Implement health checks
3. Add API documentation (Swagger/OpenAPI)
4. Implement caching strategies
5. Add deployment configuration (Docker, Azure)

## Support

For issues or questions, refer to the project documentation or contact the development team.

---

**Migration Date**: 2026-01-22
**Original Framework**: ASP.NET Web Forms 4.8
**Target Framework**: .NET 8
**Migration Tool**: Claude Code Migration Assistant
