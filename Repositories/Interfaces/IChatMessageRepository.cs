using CapstoneReviewTool.Entities;

namespace CapstoneReviewTool.Repositories.Interfaces;

public interface IChatMessageRepository
{
    Task<ChatMessage> AddAsync(ChatMessage message);
    Task<List<ChatMessage>> GetGroupHistoryAsync(int academicTermId, int capstoneGroupId, int take, int skip);
    Task<List<ChatMessage>> GetPublicHistoryAsync(int academicTermId, int take, int skip);
}
