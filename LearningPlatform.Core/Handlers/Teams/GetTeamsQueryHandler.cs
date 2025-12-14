using LearningPlatform.Common.DTOs.Teams;
using LearningPlatform.Core.Queries.Teams;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Teams;

public class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, List<TeamDto>>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;

    public GetTeamsQueryHandler(ITeamRepository teamRepository, IUserRepository userRepository)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
    }

    public async Task<List<TeamDto>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
    {
        List<Data.Entities.Team> teams;
        if (request.IsInstructor)
        {
            // Instructors see all teams in the course
            teams = await _teamRepository.GetByCourseAsync(request.CourseId, cancellationToken);
        }
        else
        {
            // Students see only teams they're members of
            teams = await _teamRepository.GetByUserAsync(request.UserId, request.CourseId, cancellationToken);
        }

        var result = new List<TeamDto>();
        foreach (var team in teams)
        {
            var memberIds = await _teamRepository.GetMemberIdsAsync(team.Id, cancellationToken);
            var members = new List<TeamMemberDto>();
            foreach (var memberId in memberIds)
            {
                var user = await _userRepository.GetByIdAsync(memberId, cancellationToken);
                if (user != null)
                {
                    members.Add(new TeamMemberDto
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        JoinedAtUtc = DateTime.UtcNow // Simplified - would need to get from TeamMembership
                    });
                }
            }

            result.Add(new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                CourseId = team.CourseId,
                Members = members,
                CreatedAtUtc = team.CreatedAtUtc
            });
        }

        return result;
    }
}

