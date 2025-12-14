namespace LearningPlatform.Data.Entities;

public class Submission
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public string Link { get; set; } = string.Empty;
    public int? Grade { get; set; }
    public string? Comment { get; set; }
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? GradedAtUtc { get; set; }
}


