using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LearningPlatform.Common.DTOs.Teams;
using LearningPlatform.Common.Enums;
using LearningPlatform.Core.Commands.Teams;
using LearningPlatform.Core.Queries.Teams;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[Route("api/courses/{courseId:guid}/teams")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetTeams([FromRoute] Guid courseId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var role = GetUserRole();
        var teams = await _mediator.Send(new GetTeamsQuery(courseId, userId, role == UserRole.Instructor), cancellationToken);
        return Ok(teams);
    }

    [Authorize(Roles = nameof(UserRole.Instructor))]
    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromRoute] Guid courseId, [FromBody] CreateTeamRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (request.CourseId != courseId)
        {
            return BadRequest(new { message = "CourseId mismatch." });
        }

        var instructorId = GetUserId();
        try
        {
            var result = await _mediator.Send(new CreateTeamCommand(instructorId, courseId, request.Name, request.StudentIds), cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = nameof(UserRole.Instructor))]
    [HttpPut("{teamId:guid}")]
    public async Task<IActionResult> UpdateTeam([FromRoute] Guid courseId, [FromRoute] Guid teamId, [FromBody] UpdateTeamRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var instructorId = GetUserId();
        try
        {
            var result = await _mediator.Send(new UpdateTeamCommand(instructorId, teamId, request.Name), cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = nameof(UserRole.Instructor))]
    [HttpDelete("{teamId:guid}")]
    public async Task<IActionResult> DeleteTeam([FromRoute] Guid courseId, [FromRoute] Guid teamId, CancellationToken cancellationToken)
    {
        var instructorId = GetUserId();
        try
        {
            await _mediator.Send(new DeleteTeamCommand(instructorId, teamId), cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = nameof(UserRole.Instructor))]
    [HttpPost("{teamId:guid}/members")]
    public async Task<IActionResult> AddMember([FromRoute] Guid courseId, [FromRoute] Guid teamId, [FromBody] AddTeamMemberRequest request, CancellationToken cancellationToken)
    {
        var instructorId = GetUserId();
        try
        {
            var result = await _mediator.Send(new AddTeamMemberCommand(instructorId, teamId, request.StudentId), cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = nameof(UserRole.Instructor))]
    [HttpDelete("{teamId:guid}/members/{studentId:guid}")]
    public async Task<IActionResult> RemoveMember([FromRoute] Guid courseId, [FromRoute] Guid teamId, [FromRoute] Guid studentId, CancellationToken cancellationToken)
    {
        var instructorId = GetUserId();
        try
        {
            var result = await _mediator.Send(new RemoveTeamMemberCommand(instructorId, teamId, studentId), cancellationToken);
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

