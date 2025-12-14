using LearningPlatform.Common.DTOs.Teams;
using MediatR;

namespace LearningPlatform.Core.Commands.Teams;

public record AddTeamMemberCommand(Guid InstructorId, Guid TeamId, Guid StudentId) : IRequest<TeamDto>;

