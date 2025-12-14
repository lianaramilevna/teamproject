using LearningPlatform.Common.DTOs.Users;
using LearningPlatform.Common.Enums;

namespace LearningPlatform.Core.Services;

public interface ITokenService
{
    (string token, DateTime expiresAtUtc) GenerateToken(Guid userId, string email, UserRole role);
}


