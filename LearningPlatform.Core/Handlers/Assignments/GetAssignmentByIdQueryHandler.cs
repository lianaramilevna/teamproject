using LearningPlatform.Common.DTOs.Assignments;
using LearningPlatform.Core.Queries.Assignments;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Assignments;

public class GetAssignmentByIdQueryHandler : IRequestHandler<GetAssignmentByIdQuery, AssignmentDto?>
{
    private readonly IAssignmentRepository _assignmentRepository;

    public GetAssignmentByIdQueryHandler(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<AssignmentDto?> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        if (assignment == null)
        {
            return null;
        }

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

