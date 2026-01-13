# DescopeSampleApp - .NET 8 Migration

This application has been successfully migrated from ASP.NET Web Forms 4.8 to .NET 8 using clean architecture principles.

## Architecture

The solution follows Clean Architecture with four main layers:

- **DescopeSampleApp.Domain**: Core entities and domain interfaces
- **DescopeSampleApp.Application**: Business logic and DTOs
- **DescopeSampleApp.Infrastructure**: External services (Descope token validation, HTTP clients)
- **DescopeSampleApp.Web**: Razor Pages UI layer

## Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or VS Code

## Configuration

Update the Descope Project ID in `appsettings.json`:

```json
{
  "Descope": {
    "ProjectId": "YOUR_DESCOPE_PROJECT_ID"
  }
}
```

## Running the Application

```bash
dotnet restore
dotnet build
dotnet run --project src/DescopeSampleApp.Web
```

The application will start at `https://localhost:5001`.

## Key Features

- **Token Validation**: Secure JWT validation using Descope SDK
- **Razor Pages**: Modern page-based UI framework
- **Dependency Injection**: Built-in DI container
- **Logging**: Serilog for structured logging
- **API Endpoints**: RESTful API with `/api/sample`

## Build Verification

Build Status: SUCCESS

- All projects compile successfully
- No errors or warnings
- Target Framework: net8.0
- All packages compatible with .NET 8
