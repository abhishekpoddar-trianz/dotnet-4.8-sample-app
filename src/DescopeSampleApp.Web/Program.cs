using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddRazorPages();
builder.Services.AddControllers();

var descopeProjectId = builder.Configuration["Descope:ProjectId"] ?? "P2dI0leWLEC45BDmfxeOCSSOWiCt";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://api.descope.com/{descopeProjectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = $"https://api.descope.com/{descopeProjectId}",
            ValidAudience = descopeProjectId,
            ClockSkew = TimeSpan.Zero
        };
        options.MetadataAddress = $"https://api.descope.com/{descopeProjectId}/.well-known/openid-configuration";
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<DescopeSampleApp.Web.Services.ITokenValidator, DescopeSampleApp.Web.Services.TokenValidator>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
