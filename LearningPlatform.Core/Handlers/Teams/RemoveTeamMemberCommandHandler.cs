using LearningPlatform.Common.DTOs.Teams;
using LearningPlatform.Core.Commands.Teams;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Teams;

public class RemoveTeamMemberCommandHandler : IRequestHandler<RemoveTeamMemberCommand, TeamDto>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public RemoveTeamMemberCommandHandler(
        ITeamRepository teamRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        _teamRepository = teamRepository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }

    public async Task<TeamDto> Handle(RemoveTeamMemberCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken);
        if (team == null)
        {
            throw new InvalidOperationException("Team not found.");
        }

        var isInstructor = await _courseRepository.IsInstructorAsync(team.CourseId, request.InstructorId, cancellationToken);
        if (!isInstructor)
        {
            throw new InvalidOperationException("Only course instructor can remove team members.");
        }

        await _teamRepository.RemoveMembershipAsync(team.Id, request.StudentId, cancellationToken);
        await _teamRepository.SaveChangesAsync(cancellationToken);

        // Load members for response
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

