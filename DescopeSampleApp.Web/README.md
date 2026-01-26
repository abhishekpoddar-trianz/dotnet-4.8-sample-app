# Descope Sample App - .NET 8 Migration

This application has been successfully migrated from ASP.NET Web Forms (.NET Framework 4.8) to ASP.NET Core (.NET 8).

## Migration Overview

The application was migrated using clean architecture principles and modern .NET 8 patterns.

### Key Changes

1. **Framework Migration**
   - Migrated from .NET Framework 4.8 to .NET 8
   - Replaced ASP.NET Web Forms with ASP.NET Core Razor Pages
   - Converted legacy project format to SDK-style project

2. **Application Architecture**
   - Moved from Web Forms pages (.aspx) to Razor Pages (.cshtml)
   - Replaced `System.Web` dependencies with ASP.NET Core equivalents
   - Implemented dependency injection throughout
   - Added structured logging with Serilog

3. **Configuration**
   - Replaced Web.config with appsettings.json
   - Migrated application settings to modern configuration system
   - Environment-specific configurations supported

4. **Authentication**
   - Maintained Descope authentication integration
   - Updated JWT validation to use ASP.NET Core authentication middleware
   - Replaced direct HttpClient instantiation with IHttpClientFactory

5. **API Controllers**
   - Migrated from Web API 2 (ApiController) to ASP.NET Core Web API (ControllerBase)
   - Updated return types from IHttpActionResult to IActionResult
   - Implemented proper async/await patterns with CancellationToken support

## Project Structure

```
DescopeSampleApp.Web/
├── Controllers/          # API Controllers
│   ├── HomeController.cs
│   └── SampleController.cs
├── Pages/               # Razor Pages
│   ├── Index.cshtml
│   ├── Login.cshtml
│   ├── Authenticated.cshtml
│   └── Error.cshtml
├── wwwroot/            # Static files
│   ├── css/
│   └── js/
├── TokenValidator.cs   # JWT token validation service
├── Program.cs          # Application entry point
├── appsettings.json    # Configuration
└── DescopeSampleApp.Web.csproj
```

## Technologies Used

- .NET 8
- ASP.NET Core Razor Pages
- ASP.NET Core Web API
- Serilog for logging
- JWT Bearer authentication
- Descope authentication

## Getting Started

### Prerequisites

- .NET 8 SDK or later
- Visual Studio 2022 (optional) or VS Code

### Running the Application

1. Navigate to the project directory:
   ```bash
   cd DescopeSampleApp.Web
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

5. Open your browser and navigate to:
   - HTTPS: https://localhost:5001
   - HTTP: http://localhost:5000

## Configuration

The application uses the following configuration in `appsettings.json`:

```json
{
  "Descope": {
    "ProjectId": "P2dI0leWLEC45BDmfxeOCSSOWiCt"
  }
}
```

You can override this with environment variables or in `appsettings.Development.json`.

## API Endpoints

### GET /api/sample
Sample API endpoint that requires Bearer token authentication.

**Headers:**
- Authorization: Bearer {token}

**Response:**
```json
{
  "message": "This is a sample API endpoint.",
  "authenticated": true,
  "timestamp": "2026-01-26T17:24:00Z"
}
```

## Pages

### /Index
Home page of the application

### /Login
Login page using Descope authentication flow

### /Authenticated
Protected page that requires authentication and demonstrates API calls

### /Error
Error page for displaying application errors

## Migration Notes

### What Was Migrated

1. **Login.aspx** → **Pages/Login.cshtml**
   - Maintained Descope web component integration
   - Updated to Razor Page format

2. **AuthenticatedPage.aspx** → **Pages/Authenticated.cshtml**
   - Maintained client-side authentication check
   - Updated to Razor Page format with Bootstrap styling

3. **Controllers/HomeController.cs**
   - Migrated from System.Web.Mvc.Controller to Microsoft.AspNetCore.Mvc.Controller
   - Updated return types and patterns

4. **Controllers/SampleController.cs**
   - Migrated from ApiController to ControllerBase
   - Updated authentication pattern to use proper async/await
   - Improved error handling and logging

5. **TokenValidator.cs**
   - Updated to use IHttpClientFactory instead of direct HttpClient instantiation
   - Added dependency injection support
   - Improved logging throughout

6. **Global.asax** → **Program.cs**
   - Application startup logic moved to Program.cs
   - Routing configuration simplified with endpoint routing
   - Middleware pipeline configured

7. **Web.config** → **appsettings.json**
   - Configuration migrated to JSON format
   - Environment-specific configurations supported

### Breaking Changes

1. **Namespace Changes**
   - `System.Web` → `Microsoft.AspNetCore`
   - `System.Web.Mvc` → `Microsoft.AspNetCore.Mvc`
   - `System.Web.Http` → `Microsoft.AspNetCore.Mvc`

2. **Authentication**
   - Client-side authentication still used (Descope SDK)
   - Server-side JWT validation updated to use ASP.NET Core patterns

3. **Static Files**
   - Moved to wwwroot directory
   - Updated references in pages

4. **Logging**
   - Console.WriteLine replaced with ILogger
   - Structured logging with Serilog

## Known Issues

None at this time.

## Future Improvements

1. Consider implementing server-side session management
2. Add comprehensive unit and integration tests
3. Implement response caching
4. Add API documentation (Swagger/OpenAPI)
5. Consider adding health check endpoints
6. Implement proper error handling middleware

## Build Verification

The application has been successfully built and verified:

- ✅ All projects compile without errors
- ✅ Zero build warnings
- ✅ All dependencies restored successfully
- ✅ Target framework: net8.0
- ✅ Nullable reference types enabled

Build Date: 2026-01-26
Build Status: SUCCESS

## License

See LICENSE file for details.

## Support

For issues or questions, please refer to the Descope documentation or contact support.
