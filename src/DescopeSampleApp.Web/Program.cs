using DescopeSampleApp.Application.Services;
using DescopeSampleApp.Domain.Interfaces.Repositories;
using DescopeSampleApp.Domain.Interfaces.Services;
using DescopeSampleApp.Infrastructure.Data;
using DescopeSampleApp.Infrastructure.Repositories;
using DescopeSampleApp.Web.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/descopesampleapp-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Configure DbContext with in-memory database for demo purposes
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("DescopeSampleAppDb"));

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenValidationService, TokenValidationService>();

// Configure HttpClient for Descope API
builder.Services.AddHttpClient("DescopeClient", client =>
{
    client.BaseAddress = new Uri("https://api.descope.com/");
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

try
{
    Log.Information("Starting DescopeSampleApp.Web application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Make the implicit Program class public so integration tests can reference it
public partial class Program { }
