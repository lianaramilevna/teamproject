using LearningPlatform.Data.Entities;

namespace LearningPlatform.Data.Repositories;

public interface ISubmissionRepository
{
    Task<Submission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Submission>> GetByAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task AddAsync(Submission submission, CancellationToken cancellationToken = default);
    Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}


