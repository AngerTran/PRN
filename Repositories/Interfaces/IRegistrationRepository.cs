using CapstoneReviewTool.Entities;

namespace CapstoneReviewTool.Repositories.Interfaces;

public interface IRegistrationRepository
{
    Task<DefenseSlotRegistration?> GetRegistrationByIdAsync(int registrationId);
    Task<List<DefenseSlotRegistration>> GetPendingRegistrationsByGroupAsync(int groupId);
    Task<List<DefenseSlotRegistration>> GetActiveRegistrationsByGroupAsync(int groupId);
    Task<bool> GroupHasAcceptedRegistrationAsync(int groupId);
    Task<DefenseSlotRegistration> CreateRegistrationAsync(DefenseSlotRegistration registration);
    Task UpdateRegistrationAsync(DefenseSlotRegistration registration);
    Task DeleteRegistrationAsync(DefenseSlotRegistration registration);
    Task<int> CountActiveRegistrationsByGroupAsync(int groupId);
    Task<bool> GroupHasRegistrationForSlotAsync(int groupId, int slotId);
}
