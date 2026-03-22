using Microsoft.AspNetCore.SignalR;
using CapstoneReviewTool.Hubs;
using CapstoneReviewTool.Services.Interfaces;

namespace CapstoneReviewTool.Services.Implementations;

public class TopicStatusNotifier : ITopicStatusNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public TopicStatusNotifier(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyGroupTopicStatusChangedAsync(int groupId, int topicId, string topicName, string newStatus, string? detail = null)
    {
        return _hubContext.Clients.Group(ChatHub.TeamGroup(groupId)).SendAsync("TopicStatusChanged", new
        {
            topicId,
            topicName,
            status = newStatus,
            detail,
            sentAtUtc = DateTime.UtcNow
        });
    }
}
