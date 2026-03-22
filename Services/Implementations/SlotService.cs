using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Repositories.Interfaces;
using CapstoneReviewTool.Services.Common;
using CapstoneReviewTool.Services.Interfaces;

namespace CapstoneReviewTool.Services.Implementations;

public class SlotService : ISlotService
{
    private readonly ISlotRepository _slotRepository;
    private readonly IRegistrationRepository _registrationRepository;

    public SlotService(ISlotRepository slotRepository, IRegistrationRepository registrationRepository)
    {
        _slotRepository = slotRepository;
        _registrationRepository = registrationRepository;
    }

    public async Task<List<DefenseSlot>> GetAvailableSlotsAsync()
    {
        var slots = await _slotRepository.GetAllSlotsWithDetailsAsync();
        
        // Filter out Cancelled registrations
        foreach (var slot in slots)
        {
            slot.Registrations = slot.Registrations
                .Where(r => r.Status != "Cancelled")
                .ToList();
        }

        return slots;
    }

    public async Task<DefenseSlot?> GetSlotDetailsAsync(int slotId)
    {
        return await _slotRepository.GetSlotWithRegistrationsAsync(slotId);
    }

    public async Task<ServiceResult<DefenseSlot>> CreateSlotAsync(DateTime date, TimeSpan startTime, TimeSpan endTime, string room, string mode, int maxGroups, int? committeeId)
    {
        var startDateTime = date.Date + startTime;
        var endDateTime = date.Date + endTime;

        if (startDateTime >= endDateTime)
        {
            return ServiceResult<DefenseSlot>.Fail("Thời gian kết thúc phải sau thời gian bắt đầu.");
        }

        var slot = new DefenseSlot
        {
            StartTime = startDateTime,
            EndTime = endDateTime,
            Room = room,
            Mode = mode,
            MaxGroups = maxGroups,
            CommitteeId = committeeId
        };

        var createdSlot = await _slotRepository.CreateSlotAsync(slot);
        return ServiceResult<DefenseSlot>.Ok(createdSlot, "Tạo slot bảo vệ thành công!");
    }

    public async Task<ServiceResult> RemoveGroupFromSlotAsync(int slotId, int groupId)
    {
        var slot = await _slotRepository.GetSlotWithRegistrationsAsync(slotId);
        if (slot == null)
        {
            return ServiceResult.Fail("Không tìm thấy slot.");
        }

        var registration = slot.Registrations.FirstOrDefault(r => r.CapstoneGroupId == groupId);
        if (registration == null)
        {
            return ServiceResult.Fail("Không tìm thấy đăng ký.");
        }

        await _registrationRepository.DeleteRegistrationAsync(registration);
        return ServiceResult.Ok("Đã loại nhóm khỏi slot.");
    }
}
