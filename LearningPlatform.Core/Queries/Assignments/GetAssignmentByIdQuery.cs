using LearningPlatform.Common.DTOs.Assignments;
using MediatR;

namespace LearningPlatform.Core.Queries.Assignments;

public record GetAssignmentByIdQuery(Guid AssignmentId) : IRequest<AssignmentDto?>;

