using LearningPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Data.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AssignmentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Assignments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Assignment?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Assignments.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Assignment>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Assignments.AsNoTracking()
            .Where(a => a.CourseId == courseId)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        await _dbContext.Assignments.AddAsync(assignment, cancellationToken);
    }

    public Task UpdateAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        _dbContext.Assignments.Update(assignment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        _dbContext.Assignments.Remove(assignment);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}


