using LearningPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Data.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CourseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Course?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Courses.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Course?> GetByJoinCodeAsync(string joinCode, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.JoinCode == joinCode, cancellationToken);
    }

    public async Task<List<Course>> GetByInstructorAsync(Guid instructorId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Courses.AsNoTracking()
            .Where(c => c.InstructorId == instructorId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Course>> GetForStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Courses.AsNoTracking()
            .Join(_dbContext.CourseMemberships,
                c => c.Id,
                m => m.CourseId,
                (c, m) => new { c, m })
            .Where(x => x.m.UserId == studentId)
            .Select(x => x.c)
            .Distinct()
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Course course, CancellationToken cancellationToken = default)
    {
        await _dbContext.Courses.AddAsync(course, cancellationToken);
    }

    public Task UpdateAsync(Course course, CancellationToken cancellationToken = default)
    {
        _dbContext.Courses.Update(course);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Course course, CancellationToken cancellationToken = default)
    {
        _dbContext.Courses.Remove(course);
        return Task.CompletedTask;
    }

    public async Task AddMembershipAsync(CourseMembership membership, CancellationToken cancellationToken = default)
    {
        await _dbContext.CourseMemberships.AddAsync(membership, cancellationToken);
    }

    public async Task<bool> IsMemberAsync(Guid courseId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CourseMemberships.AnyAsync(m => m.CourseId == courseId && m.UserId == userId, cancellationToken);
    }

    public async Task<bool> IsInstructorAsync(Guid courseId, Guid instructorId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Courses.AnyAsync(c => c.Id == courseId && c.InstructorId == instructorId, cancellationToken);
    }

    public async Task<List<Guid>> GetStudentIdsAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CourseMemberships
            .Where(m => m.CourseId == courseId)
            .Select(m => m.UserId)
            .ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}


