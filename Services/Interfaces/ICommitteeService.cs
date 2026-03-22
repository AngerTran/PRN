using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Services.Common;

namespace CapstoneReviewTool.Services.Interfaces;

public interface ICommitteeService
{
    Task<List<Committee>> GetAllCommitteesAsync();
    Task<List<ApplicationUser>> GetAllUsersAsync();
    Task<ServiceResult<Committee>> CreateCommitteeAsync(string name);
    Task<ServiceResult> AddMemberToCommitteeAsync(int committeeId, string userId);
}
