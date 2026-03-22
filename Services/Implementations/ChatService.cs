using Microsoft.AspNetCore.Identity;
using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Repositories.Interfaces;
using CapstoneReviewTool.Services.Interfaces;
using CapstoneReviewTool.ViewModels;

namespace CapstoneReviewTool.Services.Implementations;

public class ChatService : IChatService
{
    private const int MaxContentLength = 2000;
    private readonly IChatMessageRepository _messages;
    private readonly IAcademicTermRepository _terms;
    private readonly IGroupRepository _groups;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatService(
        IChatMessageRepository messages,
        IAcademicTermRepository terms,
        IGroupRepository groups,
        UserManager<ApplicationUser> userManager)
    {
        _messages = messages;
        _terms = terms;
        _groups = groups;
        _userManager = userManager;
    }

    public async Task<int?> GetActiveTermIdAsync()
    {
        var term = await _terms.GetActiveTermAsync();
        return term?.Id;
    }

    public async Task<bool> UserIsGroupMemberAsync(string userId, int groupId)
    {
        var group = await _groups.GetGroupWithMembersAsync(groupId);
        return group?.Members.Any(m => m.Id == userId) == true;
    }

    public async Task<(bool Ok, string? Error, ChatMessageViewModel? Message)> SendGroupMessageAsync(string userId, int groupId, string content)
    {
        var term = await _terms.GetActiveTermAsync();
        if (term == null)
        {
            return (false, "Chưa cấu hình kỳ học hoạt động.", null);
        }

        if (!await UserIsGroupMemberAsync(userId, groupId))
        {
            return (false, "Bạn không thuộc nhóm này.", null);
        }

        var normalized = NormalizeContent(content);
        if (normalized == null)
        {
            return (false, "Nội dung tin nhắn không hợp lệ.", null);
        }

        var entity = new ChatMessage
        {
            SenderId = userId,
            Content = normalized,
            SentAtUtc = DateTime.UtcNow,
            ChannelType = ChatChannelType.Group,
            AcademicTermId = term.Id,
            CapstoneGroupId = groupId
        };

        var saved = await _messages.AddAsync(entity);
        return (true, null, await ToViewModelAsync(saved, userId));
    }

    public async Task<(bool Ok, string? Error, ChatMessageViewModel? Message)> SendPublicMessageAsync(string userId, string content)
    {
        var term = await _terms.GetActiveTermAsync();
        if (term == null)
        {
            return (false, "Chưa cấu hình kỳ học hoạt động.", null);
        }

        var normalized = NormalizeContent(content);
        if (normalized == null)
        {
            return (false, "Nội dung tin nhắn không hợp lệ.", null);
        }

        var entity = new ChatMessage
        {
            SenderId = userId,
            Content = normalized,
            SentAtUtc = DateTime.UtcNow,
            ChannelType = ChatChannelType.Public,
            AcademicTermId = term.Id,
            CapstoneGroupId = null
        };

        var saved = await _messages.AddAsync(entity);
        return (true, null, await ToViewModelAsync(saved, userId));
    }

    public async Task<List<ChatMessageViewModel>> GetGroupHistoryAsync(int groupId, int take = 100, int skip = 0)
    {
        var term = await _terms.GetActiveTermAsync();
        if (term == null)
        {
            return new List<ChatMessageViewModel>();
        }

        var rows = await _messages.GetGroupHistoryAsync(term.Id, groupId, take, skip);
        var list = new List<ChatMessageViewModel>();
        foreach (var m in rows)
        {
            list.Add(MapToViewModel(m));
        }

        return list;
    }

    public async Task<List<ChatMessageViewModel>> GetPublicHistoryAsync(int take = 100, int skip = 0)
    {
        var term = await _terms.GetActiveTermAsync();
        if (term == null)
        {
            return new List<ChatMessageViewModel>();
        }

        var rows = await _messages.GetPublicHistoryAsync(term.Id, take, skip);
        var list = new List<ChatMessageViewModel>();
        foreach (var m in rows)
        {
            list.Add(MapToViewModel(m));
        }

        return list;
    }

    private static string? NormalizeContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        var t = content.Trim();
        return t.Length > MaxContentLength ? t[..MaxContentLength] : t;
    }

    private static ChatMessageViewModel MapToViewModel(ChatMessage m)
    {
        return new ChatMessageViewModel
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderName = m.Sender?.FullName ?? m.Sender?.UserName ?? m.SenderId,
            Content = m.Content,
            SentAtUtc = m.SentAtUtc,
            IsNotification = false
        };
    }

    private async Task<ChatMessageViewModel> ToViewModelAsync(ChatMessage m, string senderId)
    {
        var user = await _userManager.FindByIdAsync(senderId);
        return new ChatMessageViewModel
        {
            Id = m.Id,
            SenderId = senderId,
            SenderName = user?.FullName ?? user?.UserName ?? senderId,
            Content = m.Content,
            SentAtUtc = m.SentAtUtc,
            IsNotification = false
        };
    }
}
