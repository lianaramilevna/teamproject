using LearningPlatform.Common.DTOs.Courses;
using LearningPlatform.Common.Enums;
using MediatR;

namespace LearningPlatform.Core.Queries.Courses;

public record GetCourseByIdQuery(Guid CourseId, Guid UserId, UserRole Role) : IRequest<CourseDto?>;

