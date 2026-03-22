using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Repositories.Interfaces;
using CapstoneReviewTool.Services.Common;
using CapstoneReviewTool.Services.Interfaces;

namespace CapstoneReviewTool.Services.Implementations;

public class CommitteeService : ICommitteeService
{
    private readonly ICommitteeRepository _committeeRepository;

    public CommitteeService(ICommitteeRepository committeeRepository)
    {
        _committeeRepository = committeeRepository;
    }

    public async Task<List<Committee>> GetAllCommitteesAsync()
    {
        return await _committeeRepository.GetAllCommitteesWithMembersAsync();
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        return await _committeeRepository.GetAllUsersAsync();
    }

    public async Task<ServiceResult<Committee>> CreateCommitteeAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ServiceResult<Committee>.Fail("Tên hội đồng không được để trống.");
        }

        var committee = new Committee { Name = name };
        var created = await _committeeRepository.CreateCommitteeAsync(committee);
        return ServiceResult<Committee>.Ok(created);
    }

    public async Task<ServiceResult> AddMemberToCommitteeAsync(int committeeId, string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return ServiceResult.Fail("User ID không được để trống.");
        }

        var committee = await _committeeRepository.GetCommitteeByIdAsync(committeeId);
        if (committee == null)
        {
            return ServiceResult.Fail("Không tìm thấy hội đồng.");
        }

        await _committeeRepository.AddMemberToCommitteeAsync(committeeId, userId);
        return ServiceResult.Ok("Đã thêm thành viên vào hội đồng.");
    }
}
