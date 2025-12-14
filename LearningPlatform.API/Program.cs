using System.Text;
using LearningPlatform.API.Services;
using LearningPlatform.Common.Options;
using LearningPlatform.Core.Commands.Auth;
using LearningPlatform.Core.Services;
using LearningPlatform.Data;
using LearningPlatform.Data.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File("logs/learningplatform-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting Learning Platform API");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog
    builder.Host.UseSerilog();

    // Options
    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<ICourseRepository, CourseRepository>();
    builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
    builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
    builder.Services.AddScoped<ITeamRepository, TeamRepository>();
    builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
    builder.Services.AddSingleton<ITokenService, JwtTokenService>();
    builder.Services.AddScoped<DatabaseSeeder>();

    builder.Services.AddMediatR(typeof(RegisterUserCommand).Assembly);
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
            var key = Encoding.UTF8.GetBytes(jwtOptions.Key);
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

    builder.Services.AddAuthorization();

    // Application Insights (optional, skip if connection string is empty or during design-time)
    var appInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    if (!string.IsNullOrWhiteSpace(appInsightsConnectionString))
    {
        try
        {
            builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to initialize Application Insights");
        }
    }

    var app = builder.Build();

    // Apply migrations and seed database
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();
            
            // Apply pending migrations
            Log.Information("Applying database migrations...");
            await context.Database.MigrateAsync();
            Log.Information("Database migrations applied successfully");

            // Seed initial data
            var seeder = services.GetRequiredService<DatabaseSeeder>();
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            Log.Fatal(ex, "An error occurred while migrating or seeding the database");
            throw;
        }
    }

    // Use Serilog request logging
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Learning Platform API v1");
        c.RoutePrefix = "swagger";
    });

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Health check endpoint
    app.MapGet("/", () => Results.Json(new { 
        message = "Learning Platform API", 
        version = "1.0",
        endpoints = new {
            swagger = "/swagger",
            api = "/api"
        }
    }));

    Log.Information("Learning Platform API started successfully");
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
