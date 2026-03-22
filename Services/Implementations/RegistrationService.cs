using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Repositories.Interfaces;
using CapstoneReviewTool.Services.Common;
using CapstoneReviewTool.Services.Interfaces;

namespace CapstoneReviewTool.Services.Implementations;

public class RegistrationService : IRegistrationService
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly ISlotRepository _slotRepository;
    private readonly ITopicRepository _topicRepository;

    public RegistrationService(
        IRegistrationRepository registrationRepository,
        IGroupRepository groupRepository,
        ISlotRepository slotRepository,
        ITopicRepository topicRepository)
    {
        _registrationRepository = registrationRepository;
        _groupRepository = groupRepository;
        _slotRepository = slotRepository;
        _topicRepository = topicRepository;
    }

    public async Task<ServiceResult<DefenseSlotRegistration>> BookSlotAsync(string userId, int slotId)
    {
        var userGroup = await _groupRepository.GetGroupByUserIdAsync(userId);
        if (userGroup == null)
        {
            return ServiceResult<DefenseSlotRegistration>.Fail("Bạn chưa có nhóm.");
        }

        var group = await _groupRepository.GetGroupWithMembersAndTopicAsync(userGroup.Id);
        if (group == null)
        {
            return ServiceResult<DefenseSlotRegistration>.Fail("Bạn chưa có nhóm.");
        }

        if (group.LeaderId != userId)
        {
            return ServiceResult<DefenseSlotRegistration>.Fail("Chỉ nhóm trưởng mới có thể đăng ký slot.");
        }

        // Check if group has at least one topic that is Approved (tự động approve khi submit)
        var validTopics = group.Topics?.Where(t => t.Status == "Approved").ToList();
        if (validTopics == null || !validTopics.Any())
        {
            return ServiceResult<DefenseSlotRegistration>.Fail("Vui lòng có ít nhất một đề tài đã được gửi trước khi đăng ký slot bảo vệ.");
        }

        var slot = await _slotRepository.GetSlotByIdAsync(slotId);
        if (slot == null)
        {
            return ServiceResult<DefenseSlotRegistration>.Fail("Không tìm thấy slot.");
        }

        var activeRegs = slot.Registrations.Where(r => r.Status != "Cancelled").ToList();
        if (activeRegs.Count >= slot.MaxGroups)
        {
            return ServiceResult<DefenseSlotRegistration>.Fail("Slot này đã đầy.");
        }

        // Check if group already has an Accepted slot (cannot book new slots if already accepted)
        var hasApproved = await _registrationRepository.GroupHasAcceptedRegistrationAsync(group.Id);
        if (hasApproved)
        {
            return ServiceResult<DefenseSlotRegistration>.Fail("Nhóm đã có slot được duyệt, không thể đăng ký slot mới.");
        }

        var activeCount = await _registrationRepository.CountActiveRegistrationsByGroupAsync(group.Id);
        if (activeCount >= 3)
        {
            return ServiceResult<DefenseSlotRegistration>.Fail("Nhóm chỉ được đăng ký tối đa 3 slot.");
        }

        if (await _registrationRepository.GroupHasRegistrationForSlotAsync(group.Id, slotId))
        {
            return ServiceResult<DefenseSlotRegistration>.Fail("Nhóm đã đăng ký slot này rồi.");
        }

        var registration = new DefenseSlotRegistration
        {
            DefenseSlotId = slotId,
            CapstoneGroupId = group.Id,
            Status = "Pending",
            Priority = 1
        };

        var created = await _registrationRepository.CreateRegistrationAsync(registration);
        return ServiceResult<DefenseSlotRegistration>.Ok(created, "Đăng ký slot thành công!");
    }

    public async Task<ServiceResult> CancelSlotAsync(string userId, int slotId)
    {
        var group = await _groupRepository.GetGroupByUserIdAsync(userId);
        if (group == null)
        {
            return ServiceResult.Fail("Bạn chưa có nhóm.");
        }

        if (group.LeaderId != userId)
        {
            return ServiceResult.Fail("Chỉ nhóm trưởng mới có thể hủy slot.");
        }

        var slot = await _slotRepository.GetSlotByIdAsync(slotId);
        if (slot == null)
        {
            return ServiceResult.Fail("Không tìm thấy slot.");
        }

        var registration = slot.Registrations.FirstOrDefault(r => r.CapstoneGroupId == group.Id);
        if (registration == null)
        {
            return ServiceResult.Fail("Không tìm thấy đăng ký.");
        }

        if (registration.Status == "Accepted")
        {
            return ServiceResult.Fail("Slot đã được duyệt, không thể hủy.");
        }

        await _registrationRepository.DeleteRegistrationAsync(registration);
        return ServiceResult.Ok("Hủy đăng ký slot thành công.");
    }

    public async Task<ServiceResult> ApproveRegistrationAsync(int slotId, int registrationId)
    {
        var registration = await _registrationRepository.GetRegistrationByIdAsync(registrationId);
        if (registration == null)
        {
            return ServiceResult.Fail("Không tìm thấy đăng ký.");
        }

        // Ràng buộc an toàn: Check if group already has an Accepted slot
        var hasApproved = await _registrationRepository.GroupHasAcceptedRegistrationAsync(registration.CapstoneGroupId);
        if (hasApproved)
        {
            return ServiceResult.Fail("Nhóm đã được phân lịch bởi hội đồng khác.");
        }

        registration.Status = "Accepted";
        await _registrationRepository.UpdateRegistrationAsync(registration);

        // Auto-cancel other Pending slots
        var otherRegistrations = await _registrationRepository.GetPendingRegistrationsByGroupAsync(registration.CapstoneGroupId);
        foreach (var otherReg in otherRegistrations.Where(r => r.Id != registrationId))
        {
            otherReg.Status = "Cancelled";
            otherReg.CancelReason = "Auto cancel because another council approved";
            await _registrationRepository.UpdateRegistrationAsync(otherReg);
        }

        return ServiceResult.Ok("Đã cập nhật trạng thái: Accepted và hủy các đăng ký khác.");
    }

    public async Task<ServiceResult> RejectRegistrationAsync(int slotId, int registrationId)
    {
        var registration = await _registrationRepository.GetRegistrationByIdAsync(registrationId);
        if (registration == null)
        {
            return ServiceResult.Fail("Không tìm thấy đăng ký.");
        }

        registration.Status = "Rejected";
        await _registrationRepository.UpdateRegistrationAsync(registration);

        return ServiceResult.Ok("Đã từ chối đăng ký.");
    }

    public async Task<int> CountActiveRegistrationsByGroupAsync(int groupId)
    {
        return await _registrationRepository.CountActiveRegistrationsByGroupAsync(groupId);
    }

    public Task<bool> GroupHasAcceptedRegistrationAsync(int groupId)
    {
        return _registrationRepository.GroupHasAcceptedRegistrationAsync(groupId);
    }

    public async Task<string> GetDefenseScheduleSummaryForGroupAsync(int groupId)
    {
        var list = await _registrationRepository.GetActiveRegistrationsByGroupAsync(groupId);
        var accepted = list.FirstOrDefault(r => r.Status == "Accepted");
        if (accepted?.DefenseSlot != null)
        {
            var s = accepted.DefenseSlot.StartTime;
            return $"{s:dd/MM/yyyy HH:mm} · {accepted.DefenseSlot.Room}";
        }

        var pending = list.FirstOrDefault(r => r.Status == "Pending" && r.DefenseSlot != null);
        if (pending?.DefenseSlot != null)
        {
            var s = pending.DefenseSlot.StartTime;
            return $"Chờ duyệt — {s:dd/MM/yyyy HH:mm}";
        }

        return "Chưa xác định";
    }
}
