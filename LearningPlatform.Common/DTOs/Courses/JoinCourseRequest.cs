using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Common.DTOs.Courses;

public class JoinCourseRequest
{
    [Required, MaxLength(12)]
    public string JoinCode { get; set; } = string.Empty;
}


