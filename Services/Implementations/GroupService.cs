using Microsoft.AspNetCore.Identity;
using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Repositories.Interfaces;
using CapstoneReviewTool.Services.Common;
using CapstoneReviewTool.Services.Interfaces;

namespace CapstoneReviewTool.Services.Implementations;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public GroupService(IGroupRepository groupRepository, UserManager<ApplicationUser> userManager)
    {
        _groupRepository = groupRepository;
        _userManager = userManager;
    }

    public async Task<ServiceResult<CapstoneGroup>> CreateGroupAsync(string userId, string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            return ServiceResult<CapstoneGroup>.Fail("Tên nhóm không được để trống.");
        }

        // Check if user already has a group
        if (await _groupRepository.UserHasGroupAsync(userId))
        {
            return ServiceResult<CapstoneGroup>.Fail("Bạn đã có nhóm!");
        }

        var newGroup = new CapstoneGroup
        {
            Name = groupName,
            LeaderId = userId,
            Status = "Created"
        };

        var group = await _groupRepository.CreateGroupAsync(newGroup);
        await _groupRepository.AddMemberToGroupAsync(userId, group.Id);

        return ServiceResult<CapstoneGroup>.Ok(group, "Tạo nhóm thành công!");
    }

    public async Task<ServiceResult> AddMemberAsync(string leaderId, string memberEmail)
    {
        var group = await _groupRepository.GetGroupByUserIdAsync(leaderId);
        if (group == null)
        {
            return ServiceResult.Fail("Không tìm thấy nhóm.");
        }

        if (group.LeaderId != leaderId)
        {
            return ServiceResult.Fail("Chỉ nhóm trưởng mới có thể thêm thành viên.");
        }

        if (group.Members.Count >= 5)
        {
            return ServiceResult.Fail("Nhóm đã đủ thành viên.");
        }

        var memberToAdd = await _userManager.FindByEmailAsync(memberEmail);
        if (memberToAdd == null)
        {
            return ServiceResult.Fail("Không tìm thấy sinh viên với email này.");
        }

        if (memberToAdd.CapstoneGroupId != null)
        {
            return ServiceResult.Fail("Sinh viên này đã có nhóm.");
        }

        await _groupRepository.AddMemberToGroupAsync(memberToAdd.Id, group.Id);
        return ServiceResult.Ok($"Đã thêm {memberToAdd.FullName} vào nhóm.");
    }

    public async Task<ServiceResult> RemoveMemberAsync(string leaderId, string memberId)
    {
        var group = await _groupRepository.GetGroupByUserIdAsync(leaderId);
        if (group == null)
        {
            return ServiceResult.Fail("Không tìm thấy nhóm.");
        }

        if (group.LeaderId != leaderId)
        {
            return ServiceResult.Fail("Chỉ nhóm trưởng mới có thể xóa thành viên.");
        }

        if (memberId == leaderId)
        {
            return ServiceResult.Fail("Bạn không thể tự xóa chính mình.");
        }

        var member = group.Members.FirstOrDefault(m => m.Id == memberId);
        if (member == null)
        {
            return ServiceResult.Fail("Không tìm thấy thành viên.");
        }

        await _groupRepository.RemoveMemberFromGroupAsync(memberId);
        return ServiceResult.Ok("Đã xóa thành viên khỏi nhóm.");
    }

    public async Task<CapstoneGroup?> GetGroupByUserIdAsync(string userId)
    {
        return await _groupRepository.GetGroupByUserIdAsync(userId);
    }

    public async Task<CapstoneGroup?> GetGroupWithMembersAsync(int groupId)
    {
        return await _groupRepository.GetGroupWithMembersAsync(groupId);
    }
}
