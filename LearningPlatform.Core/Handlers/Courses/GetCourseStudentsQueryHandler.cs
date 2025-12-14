using LearningPlatform.Common.DTOs.Users;
using LearningPlatform.Core.Queries.Courses;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Courses;

public class GetCourseStudentsQueryHandler : IRequestHandler<GetCourseStudentsQuery, List<UserDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public GetCourseStudentsQueryHandler(
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> Handle(GetCourseStudentsQuery request, CancellationToken cancellationToken)
    {
        // Verify instructor owns the course
        var isInstructor = await _courseRepository.IsInstructorAsync(request.CourseId, request.InstructorId, cancellationToken);
        if (!isInstructor)
        {
            throw new InvalidOperationException("Only course instructor can view course students.");
        }

        // Get all students who are members of the course
        var studentIds = await _courseRepository.GetStudentIdsAsync(request.CourseId, cancellationToken);

        var students = new List<UserDto>();
        foreach (var studentId in studentIds)
        {
            var user = await _userRepository.GetByIdAsync(studentId, cancellationToken);
            if (user != null && user.Role == LearningPlatform.Common.Enums.UserRole.Student)
            {
                students.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role
                });
            }
        }

        return students.OrderBy(s => s.Email).ToList();
    }
}

