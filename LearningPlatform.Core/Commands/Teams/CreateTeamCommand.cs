using LearningPlatform.Common.DTOs.Teams;
using MediatR;

namespace LearningPlatform.Core.Commands.Teams;

public record CreateTeamCommand(Guid InstructorId, Guid CourseId, string Name, List<Guid> StudentIds) : IRequest<TeamDto>;

