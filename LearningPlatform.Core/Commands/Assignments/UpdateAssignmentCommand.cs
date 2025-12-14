using LearningPlatform.Common.DTOs.Assignments;
using MediatR;

namespace LearningPlatform.Core.Commands.Assignments;

public record UpdateAssignmentCommand(Guid InstructorId, Guid AssignmentId, string Title, string? Description, DateTime? DueDateUtc, Guid? TeamId) : IRequest<AssignmentDto>;

