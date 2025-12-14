using MediatR;

namespace LearningPlatform.Core.Commands.Assignments;

public record DeleteAssignmentCommand(Guid InstructorId, Guid AssignmentId) : IRequest<Unit>;

