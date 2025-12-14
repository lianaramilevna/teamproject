using LearningPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Data.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SubmissionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Submission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Submissions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<Submission>> GetByAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Submissions.AsNoTracking()
            .Where(s => s.AssignmentId == assignmentId)
            .OrderByDescending(s => s.SubmittedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        await _dbContext.Submissions.AddAsync(submission, cancellationToken);
    }

    public Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        _dbContext.Submissions.Update(submission);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}


