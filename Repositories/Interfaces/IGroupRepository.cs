using CapstoneReviewTool.Entities;

namespace CapstoneReviewTool.Repositories.Interfaces;

public interface IGroupRepository
{
    Task<CapstoneGroup?> GetGroupByUserIdAsync(string userId);
    Task<CapstoneGroup?> GetGroupWithMembersAsync(int groupId);
    Task<CapstoneGroup?> GetGroupWithTopicAsync(int groupId);
    Task<CapstoneGroup?> GetGroupWithMembersAndTopicAsync(int groupId);
    Task<bool> UserHasGroupAsync(string userId);
    Task<CapstoneGroup> CreateGroupAsync(CapstoneGroup group);
    Task AddMemberToGroupAsync(string userId, int groupId);
    Task RemoveMemberFromGroupAsync(string userId);
    Task UpdateGroupAsync(CapstoneGroup group);
}
