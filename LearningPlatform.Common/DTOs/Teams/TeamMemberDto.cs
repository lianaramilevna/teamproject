using LearningPlatform.Common.DTOs.Users;

namespace LearningPlatform.Common.DTOs.Teams;

public class TeamMemberDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime JoinedAtUtc { get; set; }
}

