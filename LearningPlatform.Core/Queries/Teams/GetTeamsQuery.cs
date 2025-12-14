using LearningPlatform.Common.DTOs.Teams;
using MediatR;

namespace LearningPlatform.Core.Queries.Teams;

public record GetTeamsQuery(Guid CourseId, Guid UserId, bool IsInstructor) : IRequest<List<TeamDto>>;

