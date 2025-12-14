using LearningPlatform.Client.Components;
using LearningPlatform.Client.Services;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File("logs/learningplatform-client-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting Learning Platform Client");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog
    builder.Host.UseSerilog();

    // Add services to the container.
    var razorComponentsBuilder = builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    // Enable detailed errors in development
    if (builder.Environment.IsDevelopment())
    {
        razorComponentsBuilder.AddCircuitOptions(options =>
        {
            options.DetailedErrors = true;
        });
    }

    // Explicit SignalR configuration for Blazor Server
    builder.Services.AddSignalR();

    // HttpClient for API calls
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:8080";
    var apiBaseUri = new Uri(apiBaseUrl);

    builder.Services.AddHttpClient<AuthService>(client =>
    {
        client.BaseAddress = apiBaseUri;
        client.Timeout = TimeSpan.FromSeconds(30);
    });

    builder.Services.AddHttpClient<CoursesApiService>(client =>
    {
        client.BaseAddress = apiBaseUri;
        client.Timeout = TimeSpan.FromSeconds(30);
    });

    builder.Services.AddHttpClient<AssignmentsApiService>(client =>
    {
        client.BaseAddress = apiBaseUri;
        client.Timeout = TimeSpan.FromSeconds(30);
    });

    builder.Services.AddHttpClient<SubmissionsApiService>(client =>
    {
        client.BaseAddress = apiBaseUri;
        client.Timeout = TimeSpan.FromSeconds(30);
    });

    builder.Services.AddHttpClient<UsersApiService>(client =>
    {
        client.BaseAddress = apiBaseUri;
        client.Timeout = TimeSpan.FromSeconds(30);
    });

    builder.Services.AddHttpClient<TeamsApiService>(client =>
    {
        client.BaseAddress = apiBaseUri;
        client.Timeout = TimeSpan.FromSeconds(30);
    });

    // Auth state management
    // Keep as Scoped - Singleton cannot use Scoped services (NavigationManager, IJSRuntime)
    // State will be read from localStorage on each component initialization
    builder.Services.AddScoped<AuthStateService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    // Only use HTTPS redirection if HTTPS is configured
    if (app.Configuration["ASPNETCORE_URLS"]?.Contains("https") == true)
    {
        app.UseHttpsRedirection();
    }

    app.UseRouting();
    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    Log.Information("Learning Platform Client started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
