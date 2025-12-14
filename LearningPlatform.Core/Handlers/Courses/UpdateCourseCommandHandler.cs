using LearningPlatform.Common.DTOs.Courses;
using LearningPlatform.Core.Commands.Courses;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Courses;

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, CourseDto>
{
    private readonly ICourseRepository _courseRepository;

    public UpdateCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<CourseDto> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdForUpdateAsync(request.CourseId, cancellationToken);
        if (course == null)
        {
            throw new InvalidOperationException("Course not found.");
        }

        if (course.InstructorId != request.InstructorId)
        {
            throw new InvalidOperationException("Only course instructor can update the course.");
        }

        course.Title = request.Title;
        course.Description = request.Description;
        course.UpdatedAtUtc = DateTime.UtcNow;

        await _courseRepository.UpdateAsync(course, cancellationToken);
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
}

