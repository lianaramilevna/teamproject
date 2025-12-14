using LearningPlatform.Common.DTOs.Courses;
using LearningPlatform.Common.Enums;
using LearningPlatform.Core.Queries.Courses;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Courses;

public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, List<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;

    public GetCoursesQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<List<CourseDto>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        List<Data.Entities.Course> courses;
        if (request.Role == UserRole.Instructor)
        {
            courses = await _courseRepository.GetByInstructorAsync(request.UserId, cancellationToken);
        }
        else
        {
            courses = await _courseRepository.GetForStudentAsync(request.UserId, cancellationToken);
        }

        return courses.Select(c => new CourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            JoinCode = c.JoinCode,
            InstructorId = c.InstructorId
        }).ToList();
    }
}


