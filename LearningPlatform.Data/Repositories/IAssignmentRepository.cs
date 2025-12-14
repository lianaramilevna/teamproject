using LearningPlatform.Data.Entities;

namespace LearningPlatform.Data.Repositories;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Assignment?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Assignment>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Assignment assignment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Assignment assignment, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}


