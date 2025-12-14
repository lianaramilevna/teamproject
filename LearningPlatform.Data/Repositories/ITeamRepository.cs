using LearningPlatform.Data.Entities;

namespace LearningPlatform.Data.Repositories;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Team?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Team>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<List<Team>> GetByUserAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task AddAsync(Team team, CancellationToken cancellationToken = default);
    Task UpdateAsync(Team team, CancellationToken cancellationToken = default);
    Task DeleteAsync(Team team, CancellationToken cancellationToken = default);
    Task AddMembershipAsync(TeamMembership membership, CancellationToken cancellationToken = default);
    Task RemoveMembershipAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsMemberAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetMemberIdsAsync(Guid teamId, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

