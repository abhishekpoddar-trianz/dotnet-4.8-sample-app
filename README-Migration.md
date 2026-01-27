# Descope Sample App - .NET 8 Migration

This application has been successfully migrated from ASP.NET Web Forms 4.8 to .NET 8 using clean architecture principles.

## Project Structure

The solution follows clean architecture with the following layers:

- **DescopeSampleApp.Domain**: Domain entities and interfaces
- **DescopeSampleApp.Application**: Business logic and services
- **DescopeSampleApp.Infrastructure**: Data access and external services
- **DescopeSampleApp.Web**: Razor Pages UI layer
- **DescopeSampleApp.UnitTests**: Unit tests
- **DescopeSampleApp.IntegrationTests**: Integration tests

## Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)

## Setup Instructions

1. Clone the repository
2. Navigate to the solution directory
3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
4. Update the connection string in `src/DescopeSampleApp.Web/appsettings.json`
5. Apply database migrations:
   ```bash
   dotnet ef database update --project src/DescopeSampleApp.Infrastructure --startup-project src/DescopeSampleApp.Web
   ```
6. Update Descope Project ID in `appsettings.json`

## Running the Application

```bash
dotnet run --project src/DescopeSampleApp.Web
```

The application will be available at `https://localhost:5001` (or the port specified in launchSettings.json)

## Running Tests

```bash
dotnet test
```

## Key Features

- Descope authentication integration
- Token validation using JWT
- Clean architecture with separation of concerns
- Entity Framework Core 8 for data access
- Structured logging with Serilog
- Razor Pages for UI
- RESTful API endpoints

## Migration Notes

- Replaced ASP.NET Web Forms pages (.aspx) with Razor Pages
- Migrated from System.Web to ASP.NET Core
- Replaced Web.config with appsettings.json
- Implemented dependency injection throughout
- Updated to async/await patterns
- Added comprehensive error handling and logging

## Build Verification

✅ Build Status: SUCCESS
- All projects compile without errors
- All dependencies are .NET 8 compatible
- Token validation successfully migrated
