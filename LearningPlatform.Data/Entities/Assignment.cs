namespace LearningPlatform.Data.Entities;

public class Assignment
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid? TeamId { get; set; } // Optional: if null, assignment is for entire course; if set, assignment is for specific team
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDateUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}


