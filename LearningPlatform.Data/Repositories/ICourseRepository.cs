using LearningPlatform.Data.Entities;

namespace LearningPlatform.Data.Repositories;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Course?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Course?> GetByJoinCodeAsync(string joinCode, CancellationToken cancellationToken = default);
    Task<List<Course>> GetByInstructorAsync(Guid instructorId, CancellationToken cancellationToken = default);
    Task<List<Course>> GetForStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task AddAsync(Course course, CancellationToken cancellationToken = default);
    Task UpdateAsync(Course course, CancellationToken cancellationToken = default);
    Task DeleteAsync(Course course, CancellationToken cancellationToken = default);
    Task AddMembershipAsync(CourseMembership membership, CancellationToken cancellationToken = default);
    Task<bool> IsMemberAsync(Guid courseId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsInstructorAsync(Guid courseId, Guid instructorId, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetStudentIdsAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}


