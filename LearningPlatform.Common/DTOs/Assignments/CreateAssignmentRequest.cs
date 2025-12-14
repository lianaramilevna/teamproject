using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Common.DTOs.Assignments;

public class CreateAssignmentRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }

    public DateTime? DueDateUtc { get; set; }

    public Guid? TeamId { get; set; } // Optional: if null, assignment is for entire course; if set, assignment is for specific team
}


