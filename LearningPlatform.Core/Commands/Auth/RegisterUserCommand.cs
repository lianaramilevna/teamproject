using LearningPlatform.Common.DTOs.Auth;
using MediatR;

namespace LearningPlatform.Core.Commands.Auth;

public record RegisterUserCommand(string Email, string Password, int Role) : IRequest<AuthResponse>;


