using LearningPlatform.Common.DTOs.Users;
using LearningPlatform.Core.Commands.Users;
using LearningPlatform.Core.Services;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Users;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateProfileCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        // Get user for update (not AsNoTracking)
        var user = await _userRepository.GetByIdForUpdateAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        // Verify current password
        if (!_passwordHasher.Verify(user.PasswordHash, request.CurrentPassword))
        {
            throw new InvalidOperationException("Current password is incorrect.");
        }

        // Update email if provided
        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            var emailExists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);
            if (emailExists)
            {
                throw new InvalidOperationException("Email is already taken.");
            }
            user.Email = request.Email;
        }

        // Update password if provided
        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        }

        user.UpdatedAtUtc = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role
        };
    }
}

