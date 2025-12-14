using LearningPlatform.Common.DTOs.Assignments;
using MediatR;

namespace LearningPlatform.Core.Queries.Assignments;

public record GetAssignmentsQuery(Guid CourseId) : IRequest<List<AssignmentDto>>;


