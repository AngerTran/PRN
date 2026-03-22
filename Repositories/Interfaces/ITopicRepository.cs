using CapstoneReviewTool.Entities;

namespace CapstoneReviewTool.Repositories.Interfaces;

public interface ITopicRepository
{
    Task<Topic?> GetTopicByGroupIdAsync(int groupId); // Deprecated: use GetTopicsByGroupIdAsync
    Task<List<Topic>> GetTopicsByGroupIdAsync(int groupId);
    Task<Topic?> GetTopicByIdAsync(int topicId);
    Task<Topic?> GetTopicWithGroupAsync(int topicId);
    Task<Topic?> GetTopicWithGroupAndMembersAsync(int topicId);
    Task<Topic> CreateTopicAsync(Topic topic);
    Task UpdateTopicAsync(Topic topic);
    Task DeleteTopicAsync(Topic topic);
    Task<bool> GroupHasTopicAsync(int groupId);
    Task<List<Topic>> GetAllTopicsAsync();
    Task<List<Topic>> GetTopicsByStatusAsync(string status);
    Task<List<Topic>> GetPendingTopicsAsync();
}
