using CapstoneReviewTool.ViewModels;

namespace CapstoneReviewTool.Services.Interfaces;

public interface IChatService
{
    Task<bool> UserIsGroupMemberAsync(string userId, int groupId);
    Task<(bool Ok, string? Error, ChatMessageViewModel? Message)> SendGroupMessageAsync(string userId, int groupId, string content);
    Task<(bool Ok, string? Error, ChatMessageViewModel? Message)> SendPublicMessageAsync(string userId, string content);
    Task<List<ChatMessageViewModel>> GetGroupHistoryAsync(int groupId, int take = 100, int skip = 0);
    Task<List<ChatMessageViewModel>> GetPublicHistoryAsync(int take = 100, int skip = 0);
    Task<int?> GetActiveTermIdAsync();
}
