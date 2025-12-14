namespace LearningPlatform.Data.Entities;

public class TeamMembership
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAtUtc { get; set; } = DateTime.UtcNow;
}

