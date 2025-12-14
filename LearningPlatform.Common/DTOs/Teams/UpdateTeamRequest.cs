using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Common.DTOs.Teams;

public class UpdateTeamRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}

