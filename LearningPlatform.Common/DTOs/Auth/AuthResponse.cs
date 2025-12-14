using LearningPlatform.Common.DTOs.Users;
using LearningPlatform.Common.Enums;

namespace LearningPlatform.Common.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public UserRole Role { get; set; }
    public UserDto User { get; set; } = new();
}


