using MediatR;

namespace LearningPlatform.Core.Commands.Courses;

public record DeleteCourseCommand(Guid InstructorId, Guid CourseId) : IRequest<Unit>;

