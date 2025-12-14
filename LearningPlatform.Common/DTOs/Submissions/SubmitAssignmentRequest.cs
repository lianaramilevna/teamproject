using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Common.DTOs.Submissions;

public class SubmitAssignmentRequest
{
    [Required]
    public Guid AssignmentId { get; set; }

    [Required, MaxLength(2000)]
    public string Link { get; set; } = string.Empty;
}


