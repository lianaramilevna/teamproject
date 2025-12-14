using LearningPlatform.Common.DTOs.Auth;
using LearningPlatform.Common.DTOs.Users;
using LearningPlatform.Core.Queries.Auth;
using LearningPlatform.Core.Services;
using LearningPlatform.Data.Repositories;
using MediatR;

namespace LearningPlatform.Core.Handlers.Auth;

public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUserQueryHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !_passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

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


