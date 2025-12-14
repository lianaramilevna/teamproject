using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Common.DTOs.Teams;

public class CreateTeamRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid CourseId { get; set; }

    public List<Guid> StudentIds { get; set; } = new();
}

