using LearningPlatform.Common.DTOs.Teams;
using LearningPlatform.Core.Commands.Teams;
using LearningPlatform.Data.Entities;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Teams;

public class AddTeamMemberCommandHandler : IRequestHandler<AddTeamMemberCommand, TeamDto>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public AddTeamMemberCommandHandler(
        ITeamRepository teamRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        _teamRepository = teamRepository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }

    public async Task<TeamDto> Handle(AddTeamMemberCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken);
        if (team == null)
        {
            throw new InvalidOperationException("Team not found.");
        }

        var isInstructor = await _courseRepository.IsInstructorAsync(team.CourseId, request.InstructorId, cancellationToken);
        if (!isInstructor)
        {
            throw new InvalidOperationException("Only course instructor can add team members.");
        }

        // Verify student is a member of the course
        var isMember = await _courseRepository.IsMemberAsync(team.CourseId, request.StudentId, cancellationToken);
        if (!isMember)
        {
            throw new InvalidOperationException("Student is not a member of this course.");
        }

        // Check if already a member
        var alreadyMember = await _teamRepository.IsMemberAsync(team.Id, request.StudentId, cancellationToken);
        if (alreadyMember)
        {
            throw new InvalidOperationException("Student is already a member of this team.");
        }

        var membership = new TeamMembership
        {
            Id = Guid.NewGuid(),
            TeamId = team.Id,
            UserId = request.StudentId
        };

        await _teamRepository.AddMembershipAsync(membership, cancellationToken);
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

