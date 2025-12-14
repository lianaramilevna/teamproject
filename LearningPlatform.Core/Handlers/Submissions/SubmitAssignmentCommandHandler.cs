using LearningPlatform.Common.DTOs.Submissions;
using LearningPlatform.Core.Commands.Submissions;
using LearningPlatform.Data.Entities;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Submissions;

public class SubmitAssignmentCommandHandler : IRequestHandler<SubmitAssignmentCommand, SubmissionDto>
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ISubmissionRepository _submissionRepository;

    public SubmitAssignmentCommandHandler(
        IAssignmentRepository assignmentRepository,
        ICourseRepository courseRepository,
        ISubmissionRepository submissionRepository)
    {
        _assignmentRepository = assignmentRepository;
        _courseRepository = courseRepository;
        _submissionRepository = submissionRepository;
    }

    public async Task<SubmissionDto> Handle(SubmitAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken)
            ?? throw new InvalidOperationException("Assignment not found.");

        var isMember = await _courseRepository.IsMemberAsync(assignment.CourseId, request.StudentId, cancellationToken);
        var isInstructor = await _courseRepository.IsInstructorAsync(assignment.CourseId, request.StudentId, cancellationToken);
        if (!isMember && !isInstructor)
        {
            throw new InvalidOperationException("Not allowed to submit for this course.");
        }

        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            StudentId = request.StudentId,
            Link = request.Link
        };

        await _submissionRepository.AddAsync(submission, cancellationToken);
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


