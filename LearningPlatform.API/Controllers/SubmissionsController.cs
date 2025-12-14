using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LearningPlatform.Common.DTOs.Submissions;
using LearningPlatform.Common.Enums;
using LearningPlatform.Core.Commands.Submissions;
using LearningPlatform.Core.Queries.Submissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubmissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = nameof(UserRole.Student))]
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmitAssignmentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var studentId = GetUserId();
        try
        {
            var result = await _mediator.Send(new SubmitAssignmentCommand(studentId, request.AssignmentId, request.Link), cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = nameof(UserRole.Instructor))]
    [HttpGet("/api/assignments/{assignmentId:guid}/submissions")]
    public async Task<IActionResult> GetForAssignment([FromRoute] Guid assignmentId, CancellationToken cancellationToken)
    {
        var submissions = await _mediator.Send(new GetSubmissionsQuery(assignmentId), cancellationToken);
        return Ok(submissions);
    }

    [Authorize(Roles = nameof(UserRole.Instructor))]
    [HttpPost("{submissionId:guid}/grade")]
    public async Task<IActionResult> Grade([FromRoute] Guid submissionId, [FromBody] GradeSubmissionRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var result = await _mediator.Send(new GradeSubmissionCommand(submissionId, request.Grade, request.Comment), cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
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


