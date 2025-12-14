using LearningPlatform.Core.Commands.Teams;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Teams;

public class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand, Unit>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ICourseRepository _courseRepository;

    public DeleteTeamCommandHandler(ITeamRepository teamRepository, ICourseRepository courseRepository)
    {
        _teamRepository = teamRepository;
        _courseRepository = courseRepository;
    }

    public async Task<Unit> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdForUpdateAsync(request.TeamId, cancellationToken);
        if (team == null)
        {
            throw new InvalidOperationException("Team not found.");
        }

        var isInstructor = await _courseRepository.IsInstructorAsync(team.CourseId, request.InstructorId, cancellationToken);
        if (!isInstructor)
        {
            throw new InvalidOperationException("Only course instructor can delete teams.");
        }

        await _teamRepository.DeleteAsync(team, cancellationToken);
        await _teamRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

