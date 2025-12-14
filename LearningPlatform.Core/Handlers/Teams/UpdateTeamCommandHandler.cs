using LearningPlatform.Common.DTOs.Teams;
using LearningPlatform.Core.Commands.Teams;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Teams;

public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, TeamDto>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public UpdateTeamCommandHandler(
        ITeamRepository teamRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        _teamRepository = teamRepository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }

    public async Task<TeamDto> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdForUpdateAsync(request.TeamId, cancellationToken);
        if (team == null)
        {
            throw new InvalidOperationException("Team not found.");
        }

        var isInstructor = await _courseRepository.IsInstructorAsync(team.CourseId, request.InstructorId, cancellationToken);
        if (!isInstructor)
        {
            throw new InvalidOperationException("Only course instructor can update teams.");
        }

        team.Name = request.Name;
        team.UpdatedAtUtc = DateTime.UtcNow;

        await _teamRepository.UpdateAsync(team, cancellationToken);
        await _teamRepository.SaveChangesAsync(cancellationToken);

        // Load members
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
                    JoinedAtUtc = DateTime.UtcNow
                });
            }
        }

        return new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            CourseId = team.CourseId,
            Members = members,
            CreatedAtUtc = team.CreatedAtUtc
        };
    }
}

