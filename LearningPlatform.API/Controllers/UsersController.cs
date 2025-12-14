using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LearningPlatform.Common.DTOs.Users;
using LearningPlatform.Core.Commands.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        // For now, return basic info from token claims
        // In future, could fetch from database if more fields needed
        return Ok(new
        {
            Id = userId,
            Email = User.FindFirstValue(JwtRegisteredClaimNames.Email) ?? User.FindFirstValue(ClaimTypes.Email),
            Role = User.FindFirstValue(ClaimTypes.Role)
        });
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var userId = GetUserId();
        try
        {
            var command = new UpdateProfileCommand(userId, request.Email, request.NewPassword, request.CurrentPassword);
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
}

