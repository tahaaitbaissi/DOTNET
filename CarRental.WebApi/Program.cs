using CarRental.Application;
using CarRental.Infrastructure;
using CarRental.Infrastructure.Settings;
using CarRental.Persistence;
using CarRental.WebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ================================
// 1. CONFIGURATION
// ================================
var configuration = builder.Configuration;

// ================================
// 2. REGISTER SERVICES (Layer DI)
// ================================

// Register layer-specific services
builder.Services.AddPersistenceServices(configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(configuration);

// Register seeder
builder.Services.AddScoped<CarRental.Persistence.Seed.DatabaseSeeder>();

// ================================
// 3. AUTHENTICATION (JWT)
// ================================
var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() 
    ?? new JwtSettings();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero // No tolerance for token expiration
    };

    // Add events for debugging (optional, remove in production)
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Authentication failed: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogDebug("Token validated for user: {User}", context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ================================
// 4. CORS CONFIGURATION
// ================================
var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
    ?? ["http://localhost:3000", "http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // Development policy - allows any origin
    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ================================
// 5. CONTROLLERS & API
// ================================
// Configure JSON options for Minimal APIs (used by OpenApi)
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    // Increase max depth to avoid Json serializer "CurrentDepth (64)" errors when generating OpenAPI
    options.SerializerOptions.MaxDepth = 256;
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.MaxDepth = 256;
    });

// ================================
// 6. OPENAPI / SCALAR DOCUMENTATION
// ================================
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Car Rental API";
        document.Info.Version = "v1";
        document.Info.Description = "API for managing car rentals, bookings, and vehicles";
        document.Info.Contact = new()
        {
            Name = "Car Rental Support",
            Email = "support@carrental.com"
        };
        return Task.CompletedTask;
    });
});

// ================================
// 7. EXCEPTION HANDLING
// ================================
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ================================
// BUILD THE APP
// ================================
var app = builder.Build();

// Run seeder in development environment
if (app.Environment.IsDevelopment())
{
    try
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<CarRental.Persistence.Seed.DatabaseSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// ================================
// MIDDLEWARE PIPELINE (ORDER MATTERS!)
// ================================

// 1. Exception handling (catches errors from all subsequent middleware)
app.UseExceptionHandler();

// 2. HTTPS redirection
// 2. HTTPS redirection
// app.UseHttpsRedirection();

app.UseStaticFiles();

// 3. CORS (must be before auth)
if (app.Environment.IsDevelopment())
{
    app.UseCors("Development");
}
else
{
    app.UseCors("AllowFrontend");
}

// 4. Authentication & Authorization (order matters!)
app.UseAuthentication();
app.UseAuthorization();

// 5. OpenAPI & Scalar (only in development)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Car Rental API";
        options.Theme = ScalarTheme.BluePlanet;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
        options.ShowSidebar = true;
    });
}

// 6. Map controllers
app.MapControllers();

// ================================
// STARTUP MESSAGE
// ================================
app.Logger.LogInformation("Car Rental API started");
app.Logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);

if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("Scalar API Reference available at: /scalar/v1");
    app.Logger.LogInformation("OpenAPI spec available at: /openapi/v1.json");
}

app.Run();
