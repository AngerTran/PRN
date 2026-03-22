namespace CapstoneReviewTool.Services.Interfaces;

public interface ITopicStatusNotifier
{
    Task NotifyGroupTopicStatusChangedAsync(int groupId, int topicId, string topicName, string newStatus, string? detail = null);
}
