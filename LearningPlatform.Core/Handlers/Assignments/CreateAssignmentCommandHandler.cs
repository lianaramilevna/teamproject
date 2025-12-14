using LearningPlatform.Common.DTOs.Assignments;
using LearningPlatform.Core.Commands.Assignments;
using LearningPlatform.Data.Entities;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Assignments;

public class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, AssignmentDto>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IAssignmentRepository _assignmentRepository;

    public CreateAssignmentCommandHandler(ICourseRepository courseRepository, IAssignmentRepository assignmentRepository)
    {
        _courseRepository = courseRepository;
        _assignmentRepository = assignmentRepository;
    }

    public async Task<AssignmentDto> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var isInstructor = await _courseRepository.IsInstructorAsync(request.CourseId, request.InstructorId, cancellationToken);
        if (!isInstructor)
        {
            throw new InvalidOperationException("Only course instructor can create assignments.");
        }

        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            CourseId = request.CourseId,
            TeamId = request.TeamId,
            Title = request.Title,
            Description = request.Description,
            DueDateUtc = request.DueDateUtc
        };

        await _assignmentRepository.AddAsync(assignment, cancellationToken);
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


