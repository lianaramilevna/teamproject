using LearningPlatform.Common.DTOs.Courses;
using LearningPlatform.Common.Enums;
using LearningPlatform.Core.Queries.Courses;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Courses;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, CourseDto?>
{
    private readonly ICourseRepository _courseRepository;

    public GetCourseByIdQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<CourseDto?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
        {
            return null;
        }

        // Check access: instructor can access their own courses, students can access courses they're members of
        if (request.Role == UserRole.Instructor)
        {
            if (course.InstructorId != request.UserId)
            {
                return null; // Instructor can only access their own courses
            }
        }
        else
        {
            var isMember = await _courseRepository.IsMemberAsync(course.Id, request.UserId, cancellationToken);
            if (!isMember)
            {
                return null; // Student can only access courses they're members of
            }
        }

        return new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            JoinCode = course.JoinCode,
            InstructorId = course.InstructorId
        };
    }
}

