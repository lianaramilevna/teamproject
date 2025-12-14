namespace LearningPlatform.Common.DTOs.Teams;

public class TeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public List<TeamMemberDto> Members { get; set; } = new();
    public DateTime CreatedAtUtc { get; set; }
}

