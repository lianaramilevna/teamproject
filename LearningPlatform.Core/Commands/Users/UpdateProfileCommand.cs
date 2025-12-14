using LearningPlatform.Common.DTOs.Users;
using MediatR;

namespace LearningPlatform.Core.Commands.Users;

public record UpdateProfileCommand(Guid UserId, string? Email, string? NewPassword, string CurrentPassword) : IRequest<UserDto>;

