using CapstoneReviewTool.Entities;

namespace CapstoneReviewTool.Repositories.Interfaces;

public interface ICommitteeRepository
{
    Task<List<Committee>> GetAllCommitteesWithMembersAsync();
    Task<Committee?> GetCommitteeByIdAsync(int committeeId);
    Task<Committee> CreateCommitteeAsync(Committee committee);
    Task AddMemberToCommitteeAsync(int committeeId, string userId);
    Task<List<ApplicationUser>> GetAllUsersAsync();
}
