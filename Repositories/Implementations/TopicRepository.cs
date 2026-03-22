using Microsoft.EntityFrameworkCore;
using CapstoneReviewTool.Data;
using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Repositories.Interfaces;

namespace CapstoneReviewTool.Repositories.Implementations;

public class TopicRepository : ITopicRepository
{
    private readonly ApplicationDbContext _context;

    public TopicRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Topic?> GetTopicByGroupIdAsync(int groupId)
    {
        return await _context.Topics
            .FirstOrDefaultAsync(t => t.GroupId == groupId);
    }

    public async Task<List<Topic>> GetTopicsByGroupIdAsync(int groupId)
    {
        return await _context.Topics
            .Where(t => t.GroupId == groupId)
            .OrderByDescending(t => t.SubmittedDate ?? DateTime.MinValue)
            .ThenByDescending(t => t.Id)
            .ToListAsync();
    }

    public async Task<Topic?> GetTopicWithGroupAsync(int topicId)
    {
        return await _context.Topics
            .Include(t => t.Group)
            .FirstOrDefaultAsync(t => t.Id == topicId);
    }

    public async Task<Topic> CreateTopicAsync(Topic topic)
    {
        _context.Topics.Add(topic);
        await _context.SaveChangesAsync();
        return topic;
    }

    public async Task UpdateTopicAsync(Topic topic)
    {
        _context.Topics.Update(topic);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTopicAsync(Topic topic)
    {
        _context.Topics.Remove(topic);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> GroupHasTopicAsync(int groupId)
    {
        return await _context.Topics
            .AnyAsync(t => t.GroupId == groupId);
    }

    public async Task<Topic?> GetTopicByIdAsync(int topicId)
    {
        return await _context.Topics
            .FirstOrDefaultAsync(t => t.Id == topicId);
    }

    public async Task<Topic?> GetTopicWithGroupAndMembersAsync(int topicId)
    {
        return await _context.Topics
            .Include(t => t.Group)
                .ThenInclude(g => g.Members)
            .FirstOrDefaultAsync(t => t.Id == topicId);
    }

    public async Task<List<Topic>> GetAllTopicsAsync()
    {
        return await _context.Topics
            .Include(t => t.Group)
                .ThenInclude(g => g.Leader)
            .OrderByDescending(t => t.SubmittedDate ?? DateTime.MinValue)
            .ThenByDescending(t => t.Id)
            .ToListAsync();
    }

    public async Task<List<Topic>> GetTopicsByStatusAsync(string status)
    {
        return await _context.Topics
            .Where(t => t.Status == status)
            .Include(t => t.Group)
                .ThenInclude(g => g.Leader)
            .OrderByDescending(t => t.SubmittedDate ?? DateTime.MinValue)
            .ThenByDescending(t => t.Id)
            .ToListAsync();
    }

    public async Task<List<Topic>> GetPendingTopicsAsync()
    {
        return await _context.Topics
            .Where(t => t.Status == "Submitted")
            .Include(t => t.Group)
                .ThenInclude(g => g.Leader)
            .OrderByDescending(t => t.SubmittedDate ?? DateTime.MinValue)
            .ThenByDescending(t => t.Id)
            .ToListAsync();
    }
}
