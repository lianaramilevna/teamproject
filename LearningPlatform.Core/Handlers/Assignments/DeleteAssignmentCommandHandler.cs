using LearningPlatform.Core.Commands.Assignments;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Assignments;

public class DeleteAssignmentCommandHandler : IRequestHandler<DeleteAssignmentCommand, Unit>
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ICourseRepository _courseRepository;

    public DeleteAssignmentCommandHandler(IAssignmentRepository assignmentRepository, ICourseRepository courseRepository)
    {
        _assignmentRepository = assignmentRepository;
        _courseRepository = courseRepository;
    }

    public async Task<Unit> Handle(DeleteAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdForUpdateAsync(request.AssignmentId, cancellationToken);
        if (assignment == null)
        {
            throw new InvalidOperationException("Assignment not found.");
        }

        var isInstructor = await _courseRepository.IsInstructorAsync(assignment.CourseId, request.InstructorId, cancellationToken);
        if (!isInstructor)
        {
            throw new InvalidOperationException("Only course instructor can delete the assignment.");
        }

        await _assignmentRepository.DeleteAsync(assignment, cancellationToken);
        await _assignmentRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

