namespace LearningPlatform.Data.Entities;

public class CourseMembership
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAtUtc { get; set; } = DateTime.UtcNow;
}


