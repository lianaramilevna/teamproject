using LearningPlatform.Common.DTOs.Courses;
using MediatR;

namespace LearningPlatform.Core.Commands.Courses;

public record CreateCourseCommand(Guid InstructorId, string Title, string? Description) : IRequest<CourseDto>;


