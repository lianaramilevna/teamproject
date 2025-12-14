using LearningPlatform.Core.Commands.Courses;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Courses;

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, Unit>
{
    private readonly ICourseRepository _courseRepository;

    public DeleteCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<Unit> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdForUpdateAsync(request.CourseId, cancellationToken);
        if (course == null)
        {
            throw new InvalidOperationException("Course not found.");
        }

        if (course.InstructorId != request.InstructorId)
        {
            throw new InvalidOperationException("Only course instructor can delete the course.");
        }

        await _courseRepository.DeleteAsync(course, cancellationToken);
        await _courseRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

