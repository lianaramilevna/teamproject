using LearningPlatform.Common.DTOs.Submissions;
using LearningPlatform.Core.Queries.Submissions;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Submissions;

public class GetSubmissionsQueryHandler : IRequestHandler<GetSubmissionsQuery, List<SubmissionDto>>
{
    private readonly ISubmissionRepository _submissionRepository;

    public GetSubmissionsQueryHandler(ISubmissionRepository submissionRepository)
    {
        _submissionRepository = submissionRepository;
    }

    public async Task<List<SubmissionDto>> Handle(GetSubmissionsQuery request, CancellationToken cancellationToken)
    {
        var submissions = await _submissionRepository.GetByAssignmentAsync(request.AssignmentId, cancellationToken);
        return submissions.Select(s => new SubmissionDto
        {
            Id = s.Id,
            AssignmentId = s.AssignmentId,
            StudentId = s.StudentId,
            Link = s.Link,
            Grade = s.Grade,
            Comment = s.Comment,
            SubmittedAtUtc = s.SubmittedAtUtc,
            GradedAtUtc = s.GradedAtUtc
        }).ToList();
    }
}


