using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Services.Common;

namespace CapstoneReviewTool.Services.Interfaces;

public interface ISlotService
{
    Task<List<DefenseSlot>> GetAvailableSlotsAsync();
    Task<DefenseSlot?> GetSlotDetailsAsync(int slotId);
    Task<ServiceResult<DefenseSlot>> CreateSlotAsync(DateTime date, TimeSpan startTime, TimeSpan endTime, string room, string mode, int maxGroups, int? committeeId);
    Task<ServiceResult> RemoveGroupFromSlotAsync(int slotId, int groupId);
}
