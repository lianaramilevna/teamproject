using LearningPlatform.Common.DTOs.Courses;
using LearningPlatform.Core.Commands.Courses;
using LearningPlatform.Data.Entities;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Courses;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, CourseDto>
{
    private readonly ICourseRepository _courseRepository;

    public CreateCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<CourseDto> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var joinCode = GenerateJoinCode();

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            JoinCode = joinCode,
            InstructorId = request.InstructorId
        };

        await _courseRepository.AddAsync(course, cancellationToken);
        await _courseRepository.SaveChangesAsync(cancellationToken);

        return new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            JoinCode = course.JoinCode,
            InstructorId = course.InstructorId
        };
    }

    private static string GenerateJoinCode()
    {
        return Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
    }
}


