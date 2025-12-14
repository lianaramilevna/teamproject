using LearningPlatform.Common.DTOs.Submissions;
using MediatR;

namespace LearningPlatform.Core.Commands.Submissions;

public record SubmitAssignmentCommand(Guid StudentId, Guid AssignmentId, string Link) : IRequest<SubmissionDto>;


