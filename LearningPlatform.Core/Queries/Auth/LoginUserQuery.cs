using LearningPlatform.Common.DTOs.Auth;
using MediatR;

namespace LearningPlatform.Core.Queries.Auth;

public record LoginUserQuery(string Email, string Password) : IRequest<AuthResponse>;


