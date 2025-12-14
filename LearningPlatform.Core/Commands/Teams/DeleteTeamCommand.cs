using MediatR;

namespace LearningPlatform.Core.Commands.Teams;

public record DeleteTeamCommand(Guid InstructorId, Guid TeamId) : IRequest<Unit>;

