using LearningPlatform.Common.DTOs.Teams;
using MediatR;

namespace LearningPlatform.Core.Commands.Teams;

public record UpdateTeamCommand(Guid InstructorId, Guid TeamId, string Name) : IRequest<TeamDto>;

