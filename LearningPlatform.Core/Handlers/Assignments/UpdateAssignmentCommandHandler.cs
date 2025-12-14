using LearningPlatform.Common.DTOs.Assignments;
using LearningPlatform.Core.Commands.Assignments;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Assignments;

public class UpdateAssignmentCommandHandler : IRequestHandler<UpdateAssignmentCommand, AssignmentDto>
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ICourseRepository _courseRepository;

    public UpdateAssignmentCommandHandler(IAssignmentRepository assignmentRepository, ICourseRepository courseRepository)
    {
        _assignmentRepository = assignmentRepository;
        _courseRepository = courseRepository;
    }

    public async Task<AssignmentDto> Handle(UpdateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdForUpdateAsync(request.AssignmentId, cancellationToken);
        if (assignment == null)
        {
            throw new InvalidOperationException("Assignment not found.");
        }

        var isInstructor = await _courseRepository.IsInstructorAsync(assignment.CourseId, request.InstructorId, cancellationToken);
        if (!isInstructor)
        {
            throw new InvalidOperationException("Only course instructor can update the assignment.");
        }

        assignment.Title = request.Title;
        assignment.Description = request.Description;
        assignment.DueDateUtc = request.DueDateUtc;
        assignment.TeamId = request.TeamId;

        await _assignmentRepository.UpdateAsync(assignment, cancellationToken);
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        return new AssignmentDto
        {
            Id = assignment.Id,
            CourseId = assignment.CourseId,
            TeamId = assignment.TeamId,
            Title = assignment.Title,
            Description = assignment.Description,
            DueDateUtc = assignment.DueDateUtc,
            CreatedAtUtc = assignment.CreatedAtUtc
        };
    }
}

