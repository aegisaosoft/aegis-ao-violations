using Microsoft.EntityFrameworkCore;
using HuurApi.Services;
using HuurApi.Models;

// Enable legacy timestamp behavior for Npgsql to handle DateTimes
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add HuurApi services
builder.Services.AddHuurApi(options =>
{
    var huurApiConfig = builder.Configuration.GetSection("HuurApi");
    options.BaseUrl = huurApiConfig["BaseUrl"] ?? "https://agsm-huur-production-api.azurewebsites.net";
    options.DocumentationUrl = huurApiConfig["DocumentationUrl"] ?? "https://agsm-huur-production-api.azurewebsites.net/swagger/index.html";
    options.TimeoutSeconds = huurApiConfig.GetValue<int>("TimeoutSeconds", 30);
    options.MaxRetries = huurApiConfig.GetValue<int>("MaxRetries", 3);
    options.RetryDelayMs = huurApiConfig.GetValue<int>("RetryDelayMs", 1000);
    options.PersistTokens = huurApiConfig.GetValue<bool>("PersistTokens", true);
    options.TokenConfigPath = huurApiConfig["TokenConfigPath"] ?? "huur-api-config.json";
    options.ApiKey = huurApiConfig["ApiKey"]; // Optional API key
});

// Add Database Configuration Service
builder.Services.AddScoped<AegisViolations.Services.IDatabaseConfigService, AegisViolations.Services.DatabaseConfigService>();

// Add Memory Cache for token caching
builder.Services.AddMemoryCache();

// Add Huur API Authentication Service
builder.Services.AddScoped<AegisViolations.Services.IHuurApiAuthService, AegisViolations.Services.HuurApiAuthService>();

// Add Progress Tracking Service (Singleton for in-memory storage)
builder.Services.AddSingleton<AegisViolations.Services.IProgressTrackingService, AegisViolations.Services.ProgressTrackingService>();

// Add Entity Framework with configuration
builder.Services.AddDbContext<AegisViolations.Data.ViolationsDbContext>((serviceProvider, options) =>
{
    try
    {
        var dbConfigService = serviceProvider.GetRequiredService<AegisViolations.Services.IDatabaseConfigService>();
        var connectionString = dbConfigService.GetConnectionString();
        var dbSettings = dbConfigService.GetDatabaseSettings();

        var logger = serviceProvider.GetRequiredService<ILogger<AegisViolations.Data.ViolationsDbContext>>();
        logger.LogInformation("[Database] Configuring ViolationsDbContext...");

        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.CommandTimeout(dbSettings.CommandTimeout);
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: dbSettings.MaxRetryCount,
                maxRetryDelay: TimeSpan.Parse(dbSettings.MaxRetryDelay),
                errorCodesToAdd: null);
        });

        if (dbSettings.EnableSensitiveDataLogging)
        {
            options.EnableSensitiveDataLogging();
        }

        if (dbSettings.EnableDetailedErrors)
        {
            options.EnableDetailedErrors();
        }

        if (dbSettings.EnableServiceProviderCaching)
        {
            options.EnableServiceProviderCaching();
        }

        logger.LogInformation("[Database] ViolationsDbContext configured successfully. CommandTimeout: {Timeout}s, RetryOnFailure: {RetryEnabled}, MaxRetryCount: {MaxRetry}",
            dbSettings.CommandTimeout, dbSettings.EnableRetryOnFailure, dbSettings.MaxRetryCount);
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<AegisViolations.Data.ViolationsDbContext>>();
        logger.LogCritical(ex, "Failed to configure ViolationsDbContext");
        throw;
    }
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Aegis Violations API",
        Version = "v1",
        Description = "API for managing violations"
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Make Swagger available in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Aegis Violations API v1");
    c.RoutePrefix = "swagger"; // Swagger UI will be available at /swagger
    c.DisplayRequestDuration();
});
app.MapOpenApi();

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowLocalhost3000");

app.UseAuthorization();

app.MapControllers();

app.Run();
