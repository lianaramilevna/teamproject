using System.ComponentModel.DataAnnotations;
using LearningPlatform.Common.Enums;

namespace LearningPlatform.Common.DTOs.Auth;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    // По умолчанию Student; можно указать Instructor только админом (валидация в хендлере)
    public UserRole Role { get; set; } = UserRole.Student;
}


