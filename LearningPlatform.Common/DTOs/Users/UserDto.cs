using LearningPlatform.Common.Enums;

namespace LearningPlatform.Common.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}


