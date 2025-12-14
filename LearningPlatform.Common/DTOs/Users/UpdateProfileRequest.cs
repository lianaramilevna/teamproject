using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Common.DTOs.Users;

public class UpdateProfileRequest
{
    [EmailAddress]
    [MaxLength(256)]
    public string? Email { get; set; }

    [MinLength(6)]
    [MaxLength(100)]
    public string? NewPassword { get; set; }

    [Required]
    [MinLength(6)]
    public string CurrentPassword { get; set; } = string.Empty;
}

