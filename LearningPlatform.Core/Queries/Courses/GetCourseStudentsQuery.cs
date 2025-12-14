using LearningPlatform.Common.DTOs.Users;
using MediatR;

namespace LearningPlatform.Core.Queries.Courses;

public record GetCourseStudentsQuery(Guid CourseId, Guid InstructorId) : IRequest<List<UserDto>>;

