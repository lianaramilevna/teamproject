using LearningPlatform.Common.DTOs.Courses;
using MediatR;

namespace LearningPlatform.Core.Commands.Courses;

public record UpdateCourseCommand(Guid InstructorId, Guid CourseId, string Title, string? Description) : IRequest<CourseDto>;

