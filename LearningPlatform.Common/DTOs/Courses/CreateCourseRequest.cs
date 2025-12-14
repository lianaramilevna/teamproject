using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Common.DTOs.Courses;

public class CreateCourseRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }
}


