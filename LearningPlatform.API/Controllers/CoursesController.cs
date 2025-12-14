using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LearningPlatform.Common.DTOs.Courses;
using LearningPlatform.Common.Enums;
using LearningPlatform.Core.Commands.Courses;
using LearningPlatform.Core.Queries.Courses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyCourses(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var role = GetUserRole();

        var courses = await _mediator.Send(new GetCoursesQuery(userId, role), cancellationToken);
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourseById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var role = GetUserRole();

        var course = await _mediator.Send(new GetCourseByIdQuery(id, userId, role), cancellationToken);
        if (course == null)
        {
            return NotFound(new { message = "Course not found or you don't have access to it." });
        }
        return Ok(course);
    }

    [Authorize(Roles = nameof(UserRole.Instructor))]
    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var instructorId = GetUserId();
        var command = new CreateCourseCommand(instructorId, request.Title, request.Description);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = nameof(UserRole.Instructor))]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var instructorId = GetUserId();
        try
        {
            var command = new UpdateCourseCommand(instructorId, id, request.Title, request.Description);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = nameof(UserRole.Instructor))]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(Guid id, CancellationToken cancellationToken)
    {
        var instructorId = GetUserId();
        try
        {
            var command = new DeleteCourseCommand(instructorId, id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = nameof(UserRole.Instructor))]
    [HttpGet("{id}/students")]
    public async Task<IActionResult> GetCourseStudents(Guid id, CancellationToken cancellationToken)
    {
        var instructorId = GetUserId();
        try
        {
            var students = await _mediator.Send(new GetCourseStudentsQuery(id, instructorId), cancellationToken);
            return Ok(students);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = nameof(UserRole.Student))]
    [HttpPost("join")]
    public async Task<IActionResult> JoinCourse([FromBody] JoinCourseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var userId = GetUserId();
        var command = new JoinCourseCommand(userId, request.JoinCode);
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private Guid GetUserId()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(sub, out var id) ? id : throw new InvalidOperationException("User id is missing.");
    }

    private UserRole GetUserRole()
    {
        var roleClaim = User.FindFirstValue(ClaimTypes.Role);
        return Enum.TryParse<UserRole>(roleClaim, out var role) ? role : UserRole.Student;
    }
}


