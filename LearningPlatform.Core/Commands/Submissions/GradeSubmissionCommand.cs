using LearningPlatform.Common.DTOs.Submissions;
using MediatR;

namespace LearningPlatform.Core.Commands.Submissions;

public class GradeSubmissionCommand : IRequest<SubmissionDto>
{
    public Guid SubmissionId { get; }
    public int Grade { get; }
    public string? Comment { get; }

    public GradeSubmissionCommand(Guid submissionId, int grade, string? comment)
    {
        SubmissionId = submissionId;
        Grade = grade;
        Comment = comment;
    }
}

