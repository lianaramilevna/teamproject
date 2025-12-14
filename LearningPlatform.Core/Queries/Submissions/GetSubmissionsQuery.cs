using LearningPlatform.Common.DTOs.Submissions;
using MediatR;

namespace LearningPlatform.Core.Queries.Submissions;

public record GetSubmissionsQuery(Guid AssignmentId) : IRequest<List<SubmissionDto>>;


