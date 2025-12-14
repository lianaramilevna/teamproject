using LearningPlatform.Common.Enums;
using LearningPlatform.Core.Services;
using LearningPlatform.Data;
using LearningPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.API.Services;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Проверяем, есть ли уже пользователи
            var hasUsers = await _context.Users.AnyAsync();
            if (hasUsers)
            {
                _logger.LogInformation("Database already has users. Skipping seed data.");
                return;
            }

            _logger.LogInformation("Seeding initial users...");

            // Создаем студента
            var student = new User
            {
                Id = Guid.NewGuid(),
                Email = "tst@tst.com",
                PasswordHash = _passwordHasher.Hash("qweasd"),
                Role = UserRole.Student,
                CreatedAtUtc = DateTime.UtcNow
            };

            // Создаем инструктора
            var instructor = new User
            {
                Id = Guid.NewGuid(),
                Email = "inst@inst.com",
                PasswordHash = _passwordHasher.Hash("qweasd"),
                Role = UserRole.Instructor,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _context.Users.AddAsync(student);
            await _context.Users.AddAsync(instructor);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully seeded initial users:");
            _logger.LogInformation("  - Student: tst@tst.com (password: qweasd)");
            _logger.LogInformation("  - Instructor: inst@inst.com (password: qweasd)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding database");
            throw;
        }
    }
}
