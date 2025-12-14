using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LearningPlatform.Common.DTOs.Assignments;
using LearningPlatform.Common.Enums;
using LearningPlatform.Core.Commands.Assignments;
using LearningPlatform.Core.Queries.Assignments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[Route("api/courses/{courseId:guid}/assignments")]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssignmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAssignments([FromRoute] Guid courseId, CancellationToken cancellationToken)
    {
        var assignments = await _mediator.Send(new GetAssignmentsQuery(courseId), cancellationToken);
        return Ok(assignments);
    }

    [HttpGet("{assignmentId:guid}")]
    public async Task<IActionResult> GetAssignmentById([FromRoute] Guid courseId, [FromRoute] Guid assignmentId, CancellationToken cancellationToken)
    {
        var assignment = await _mediator.Send(new GetAssignmentByIdQuery(assignmentId), cancellationToken);
        if (assignment == null)
        {
            return NotFound(new { message = "Assignment not found." });
        }
        return Ok(assignment);
    }

    [Authorize(Roles = nameof(UserRole.Instructor))]
    [HttpPost]
    public async Task<IActionResult> CreateAssignment([FromRoute] Guid courseId, [FromBody] CreateAssignmentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var instructorId = GetUserId();
        try
        {
            var result = await _mediator.Send(new CreateAssignmentCommand(instructorId, courseId, request.Title, request.Description, request.DueDateUtc, request.TeamId), cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = nameof(UserRole.Instructor))]
    [HttpPut("{assignmentId:guid}")]
    public async Task<IActionResult> UpdateAssignment([FromRoute] Guid courseId, [FromRoute] Guid assignmentId, [FromBody] UpdateAssignmentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var instructorId = GetUserId();
        try
        {
            var result = await _mediator.Send(new UpdateAssignmentCommand(instructorId, assignmentId, request.Title, request.Description, request.DueDateUtc, request.TeamId), cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = nameof(UserRole.Instructor))]
    [HttpDelete("{assignmentId:guid}")]
    public async Task<IActionResult> DeleteAssignment([FromRoute] Guid courseId, [FromRoute] Guid assignmentId, CancellationToken cancellationToken)
    {
        var instructorId = GetUserId();
        try
        {
            await _mediator.Send(new DeleteAssignmentCommand(instructorId, assignmentId), cancellationToken);
            return NoContent();
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
}


