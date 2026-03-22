using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using CapstoneReviewTool.Services.Interfaces;

namespace CapstoneReviewTool.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public const string TeamPrefix = "team-";
    public const string PublicPrefix = "public-";

    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public static string TeamGroup(int teamId) => $"{TeamPrefix}{teamId}";
    public static string PublicGroup(int termId) => $"{PublicPrefix}{termId}";

    public async Task JoinTeamRoom(int teamId)
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
        {
            throw new HubException("Chưa đăng nhập.");
        }

        if (!await _chatService.UserIsGroupMemberAsync(userId, teamId))
        {
            throw new HubException("Bạn không phải thành viên nhóm này.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, TeamGroup(teamId));
    }

    public async Task JoinPublicRoom(int termId)
    {
        if (!Context.User?.IsInRole("Student") ?? true)
        {
            throw new HubException("Chỉ sinh viên tham gia kênh cộng đồng.");
        }

        var activeId = await _chatService.GetActiveTermIdAsync();
        if (activeId == null || activeId != termId)
        {
            throw new HubException("Kỳ học không hợp lệ hoặc chưa được kích hoạt.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, PublicGroup(termId));
    }

    public async Task SendTeamMessage(int teamId, string content)
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
        {
            throw new HubException("Chưa đăng nhập.");
        }

        var (ok, error, message) = await _chatService.SendGroupMessageAsync(userId, teamId, content);
        if (!ok || message == null)
        {
            throw new HubException(error ?? "Không gửi được tin nhắn.");
        }

        await Clients.Group(TeamGroup(teamId)).SendAsync("ReceiveGroupMessage", message);
    }

    public async Task SendPublicMessage(int termId, string content)
    {
        if (!Context.User?.IsInRole("Student") ?? true)
        {
            throw new HubException("Chỉ sinh viên gửi tin lên kênh cộng đồng.");
        }

        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
        {
            throw new HubException("Chưa đăng nhập.");
        }

        var (ok, error, message) = await _chatService.SendPublicMessageAsync(userId, content);
        if (!ok || message == null)
        {
            throw new HubException(error ?? "Không gửi được tin nhắn.");
        }

        await Clients.Group(PublicGroup(termId)).SendAsync("ReceivePublicMessage", message);
    }
}
