using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Services.Common;

namespace CapstoneReviewTool.Services.Interfaces;

public interface IRegistrationService
{
    Task<ServiceResult<DefenseSlotRegistration>> BookSlotAsync(string userId, int slotId);
    Task<ServiceResult> CancelSlotAsync(string userId, int slotId);
    Task<ServiceResult> ApproveRegistrationAsync(int slotId, int registrationId);
    Task<ServiceResult> RejectRegistrationAsync(int slotId, int registrationId);
    Task<int> CountActiveRegistrationsByGroupAsync(int groupId);
    Task<bool> GroupHasAcceptedRegistrationAsync(int groupId);
    Task<string> GetDefenseScheduleSummaryForGroupAsync(int groupId);
}
