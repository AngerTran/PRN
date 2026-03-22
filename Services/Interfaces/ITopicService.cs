using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Services.Common;

namespace CapstoneReviewTool.Services.Interfaces;

public interface ITopicService
{
    Task<ServiceResult<Topic>> CreateTopicAsync(string userId, int groupId, string name, string description, string field);
    Task<ServiceResult> UpdateTopicAsync(string userId, int topicId, string name, string description, string field, string? repoLink);
    Task<ServiceResult> DeleteTopicAsync(string userId, int topicId);
    Task<ServiceResult> SubmitTopicAsync(string userId, int topicId);
    Task<ServiceResult> ApproveTopicAsync(int topicId, string? comment = null);
    Task<ServiceResult> RejectTopicAsync(int topicId, string? rejectionReason = null);
    Task<ServiceResult> RequestRevisionAsync(int topicId, string? comment = null);
    Task<Topic?> GetTopicByGroupIdAsync(int groupId); // Deprecated: use GetTopicsByGroupIdAsync
    Task<List<Topic>> GetTopicsByGroupIdAsync(int groupId);
    Task<Topic?> GetTopicByIdAsync(int topicId);
    Task<List<Topic>> GetAllTopicsAsync();
    Task<List<Topic>> GetTopicsByStatusAsync(string status);
    Task<List<Topic>> GetPendingTopicsAsync();
}
