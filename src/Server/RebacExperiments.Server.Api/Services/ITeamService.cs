using RebacExperiments.Server.Database.Models;

namespace RebacExperiments.Server.Api.Services
{
    public interface ITeamService
    {
        Task<TeamRole> AddUserToTeamAsync(int teamId, int userId, int currentUserId, CancellationToken cancellationToken);
        Task<Team> CreateTeamAsync(Team team, int currentUserId, CancellationToken cancellationToken);
        Task DeleteTeamAsync(int teamId, int currentUserId, CancellationToken cancellationToken);
        Task<Team> GetTeamByIdAsync(int teamId, int currentUserId, CancellationToken cancellationToken);
        Task<List<Team>> GetTeamsByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task RemoveUserFromTeamAsync(int teamId, int userId, int currentUserId, CancellationToken cancellationToken);
        Task<Team> UpdateTeamAsync(Team team, int currentUserId, CancellationToken cancellationToken);
    }
}