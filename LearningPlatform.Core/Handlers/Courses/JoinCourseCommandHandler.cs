using LearningPlatform.Common.DTOs.Courses;
using LearningPlatform.Core.Commands.Courses;
using LearningPlatform.Data.Entities;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Courses;

public class JoinCourseCommandHandler : IRequestHandler<JoinCourseCommand, CourseDto>
{
    private readonly ICourseRepository _courseRepository;

    public JoinCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<CourseDto> Handle(JoinCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByJoinCodeAsync(request.JoinCode, cancellationToken)
            ?? throw new InvalidOperationException("Course not found.");

        var alreadyMember = await _courseRepository.IsMemberAsync(course.Id, request.UserId, cancellationToken);
        if (!alreadyMember)
        {
            var membership = new CourseMembership
            {
                Id = Guid.NewGuid(),
                CourseId = course.Id,
                UserId = request.UserId
            };
            await _courseRepository.AddMembershipAsync(membership, cancellationToken);
            await _courseRepository.SaveChangesAsync(cancellationToken);
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


