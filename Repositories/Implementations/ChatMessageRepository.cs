using Microsoft.EntityFrameworkCore;
using CapstoneReviewTool.Data;
using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Repositories.Interfaces;

namespace CapstoneReviewTool.Repositories.Implementations;

public class ChatMessageRepository : IChatMessageRepository
{
    private readonly ApplicationDbContext _context;

    public ChatMessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ChatMessage> AddAsync(ChatMessage message)
    {
        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<List<ChatMessage>> GetGroupHistoryAsync(int academicTermId, int capstoneGroupId, int take, int skip)
    {
        var list = await _context.ChatMessages
            .AsNoTracking()
            .Include(m => m.Sender)
            .Where(m => m.AcademicTermId == academicTermId
                        && m.ChannelType == ChatChannelType.Group
                        && m.CapstoneGroupId == capstoneGroupId)
            .OrderByDescending(m => m.SentAtUtc)
            .ThenByDescending(m => m.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
        list.Reverse();
        return list;
    }

    public async Task<List<ChatMessage>> GetPublicHistoryAsync(int academicTermId, int take, int skip)
    {
        var list = await _context.ChatMessages
            .AsNoTracking()
            .Include(m => m.Sender)
            .Where(m => m.AcademicTermId == academicTermId && m.ChannelType == ChatChannelType.Public)
            .OrderByDescending(m => m.SentAtUtc)
            .ThenByDescending(m => m.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
        list.Reverse();
        return list;
    }
}
