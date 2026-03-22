using CapstoneReviewTool.Entities;

namespace CapstoneReviewTool.Repositories.Interfaces;

public interface ISlotRepository
{
    Task<List<DefenseSlot>> GetAllSlotsWithDetailsAsync();
    Task<DefenseSlot?> GetSlotByIdAsync(int slotId);
    Task<DefenseSlot?> GetSlotWithRegistrationsAsync(int slotId);
    Task<DefenseSlot> CreateSlotAsync(DefenseSlot slot);
    Task UpdateSlotAsync(DefenseSlot slot);
    Task<List<DefenseSlot>> GetSlotsByCommitteeIdAsync(int? committeeId);
}
