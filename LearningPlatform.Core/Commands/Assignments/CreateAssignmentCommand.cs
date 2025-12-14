using LearningPlatform.Common.DTOs.Assignments;
using MediatR;

namespace LearningPlatform.Core.Commands.Assignments;

public record CreateAssignmentCommand(Guid InstructorId, Guid CourseId, string Title, string? Description, DateTime? DueDateUtc, Guid? TeamId) : IRequest<AssignmentDto>;


