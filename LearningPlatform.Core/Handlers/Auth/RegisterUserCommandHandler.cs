using LearningPlatform.Common.DTOs.Auth;
using LearningPlatform.Common.DTOs.Users;
using LearningPlatform.Common.Enums;
using LearningPlatform.Core.Commands.Auth;
using LearningPlatform.Core.Services;
using LearningPlatform.Data.Entities;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Auth;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var role = (UserRole)request.Role;

        // Ограничиваем регистрацию ролей: студент по умолчанию; остальные — позже по админ-потоку
        if (role is not UserRole.Student)
        {
            throw new InvalidOperationException("Only student self-registration is allowed.");
        }

        var exists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = role
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var (token, expiresAtUtc) = _tokenService.GenerateToken(user.Id, user.Email, user.Role);

        return new AuthResponse
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            Role = user.Role,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            }
        };
    }
}


