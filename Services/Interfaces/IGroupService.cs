using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Services.Common;

namespace CapstoneReviewTool.Services.Interfaces;

public interface IGroupService
{
    Task<ServiceResult<CapstoneGroup>> CreateGroupAsync(string userId, string groupName);
    Task<ServiceResult> AddMemberAsync(string leaderId, string memberEmail);
    Task<ServiceResult> RemoveMemberAsync(string leaderId, string memberId);
    Task<CapstoneGroup?> GetGroupByUserIdAsync(string userId);
    Task<CapstoneGroup?> GetGroupWithMembersAsync(int groupId);
}
