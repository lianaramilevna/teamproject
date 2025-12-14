using LearningPlatform.Common.DTOs.Courses;
using MediatR;

namespace LearningPlatform.Core.Commands.Courses;

public record JoinCourseCommand(Guid UserId, string JoinCode) : IRequest<CourseDto>;


