using LearningPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Data.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TeamRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Teams.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Team?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Teams.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<Team>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Teams.AsNoTracking()
            .Where(t => t.CourseId == courseId)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Team>> GetByUserAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Teams.AsNoTracking()
            .Join(_dbContext.TeamMemberships,
                t => t.Id,
                m => m.TeamId,
                (t, m) => new { t, m })
            .Where(x => x.m.UserId == userId && x.t.CourseId == courseId)
            .Select(x => x.t)
            .Distinct()
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        await _dbContext.Teams.AddAsync(team, cancellationToken);
    }

    public Task UpdateAsync(Team team, CancellationToken cancellationToken = default)
    {
        _dbContext.Teams.Update(team);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Team team, CancellationToken cancellationToken = default)
    {
        _dbContext.Teams.Remove(team);
        return Task.CompletedTask;
    }

    public async Task AddMembershipAsync(TeamMembership membership, CancellationToken cancellationToken = default)
    {
        await _dbContext.TeamMemberships.AddAsync(membership, cancellationToken);
    }

    public async Task RemoveMembershipAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default)
    {
        var membership = await _dbContext.TeamMemberships
            .FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == userId, cancellationToken);
        if (membership != null)
        {
            _dbContext.TeamMemberships.Remove(membership);
        }
    }

    public async Task<bool> IsMemberAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TeamMemberships.AnyAsync(m => m.TeamId == teamId && m.UserId == userId, cancellationToken);
    }

    public async Task<List<Guid>> GetMemberIdsAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TeamMemberships
            .Where(m => m.TeamId == teamId)
            .Select(m => m.UserId)
            .ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}

