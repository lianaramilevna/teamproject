using LearningPlatform.Common.DTOs.Courses;
using LearningPlatform.Common.Enums;
using MediatR;

namespace LearningPlatform.Core.Queries.Courses;

public record GetCoursesQuery(Guid UserId, UserRole Role) : IRequest<List<CourseDto>>;


