using LearningPlatform.Common.DTOs.Assignments;
using LearningPlatform.Core.Queries.Assignments;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Assignments;

public class GetAssignmentsQueryHandler : IRequestHandler<GetAssignmentsQuery, List<AssignmentDto>>
{
    private readonly IAssignmentRepository _assignmentRepository;

    public GetAssignmentsQueryHandler(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<List<AssignmentDto>> Handle(GetAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentRepository.GetByCourseAsync(request.CourseId, cancellationToken);
        return assignments.Select(a => new AssignmentDto
        {
            Id = a.Id,
            CourseId = a.CourseId,
            TeamId = a.TeamId,
            Title = a.Title,
            Description = a.Description,
            DueDateUtc = a.DueDateUtc,
            CreatedAtUtc = a.CreatedAtUtc
        }).ToList();
    }
}


