using LearningPlatform.Common.DTOs.Submissions;
using LearningPlatform.Core.Commands.Submissions;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Submissions;

public class GradeSubmissionCommandHandler : IRequestHandler<GradeSubmissionCommand, SubmissionDto>
{
    private readonly ISubmissionRepository _submissionRepository;

    public GradeSubmissionCommandHandler(ISubmissionRepository submissionRepository)
    {
        _submissionRepository = submissionRepository;
    }

    public async Task<SubmissionDto> Handle(GradeSubmissionCommand request, CancellationToken cancellationToken)
    {
        var submission = await _submissionRepository.GetByIdAsync(request.SubmissionId, cancellationToken);
        if (submission == null)
        {
            throw new InvalidOperationException($"Submission with id {request.SubmissionId} not found.");
        }

        if (request.Grade < 0 || request.Grade > 100)
        {
            throw new ArgumentException("Grade must be between 0 and 100.");
        }

        submission.Grade = request.Grade;
        submission.Comment = request.Comment;
        submission.GradedAtUtc = DateTime.UtcNow;

        await _submissionRepository.UpdateAsync(submission, cancellationToken);
        await _submissionRepository.SaveChangesAsync(cancellationToken);

        return new SubmissionDto
        {
            Id = submission.Id,
            AssignmentId = submission.AssignmentId,
            StudentId = submission.StudentId,
            Link = submission.Link,
            Grade = submission.Grade,
            Comment = submission.Comment,
            SubmittedAtUtc = submission.SubmittedAtUtc,
            GradedAtUtc = submission.GradedAtUtc
        };
    }
}


