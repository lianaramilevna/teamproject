using LearningPlatform.BackgroundTasks;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File("logs/learningplatform-backgroundtasks-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting Learning Platform Background Tasks");

    var builder = Host.CreateApplicationBuilder(args);
    
    // Use Serilog
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();
    
    builder.Services.AddHostedService<Worker>();

    var host = builder.Build();
    
    Log.Information("Learning Platform Background Tasks started successfully");
    host.Run();
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
