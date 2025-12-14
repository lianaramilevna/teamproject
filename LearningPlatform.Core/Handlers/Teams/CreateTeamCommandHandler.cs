using LearningPlatform.Common.DTOs.Teams;
using LearningPlatform.Core.Commands.Teams;
using LearningPlatform.Data.Entities;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Teams;

public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, TeamDto>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public CreateTeamCommandHandler(
        ITeamRepository teamRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        _teamRepository = teamRepository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }

    public async Task<TeamDto> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        // Verify instructor owns the course
        var isInstructor = await _courseRepository.IsInstructorAsync(request.CourseId, request.InstructorId, cancellationToken);
        if (!isInstructor)
        {
            throw new InvalidOperationException("Only course instructor can create teams.");
        }

        // Verify all students are members of the course
        foreach (var studentId in request.StudentIds)
        {
            var isMember = await _courseRepository.IsMemberAsync(request.CourseId, studentId, cancellationToken);
            if (!isMember)
            {
                throw new InvalidOperationException($"Student {studentId} is not a member of this course.");
            }
        }

        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CourseId = request.CourseId
        };

        await _teamRepository.AddAsync(team, cancellationToken);

        // Add team memberships
        foreach (var studentId in request.StudentIds)
        {
            var membership = new TeamMembership
            {
                Id = Guid.NewGuid(),
                TeamId = team.Id,
                UserId = studentId
            };
            await _teamRepository.AddMembershipAsync(membership, cancellationToken);
        }

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
                    JoinedAtUtc = DateTime.UtcNow // Will be set correctly when we load from DB
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

