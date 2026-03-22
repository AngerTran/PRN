using Microsoft.EntityFrameworkCore;
using CapstoneReviewTool.Data;
using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Repositories.Interfaces;

namespace CapstoneReviewTool.Repositories.Implementations;

public class GroupRepository : IGroupRepository
{
    private readonly ApplicationDbContext _context;

    public GroupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CapstoneGroup?> GetGroupByUserIdAsync(string userId)
    {
        return await _context.CapstoneGroups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Members.Any(m => m.Id == userId));
    }

    public async Task<CapstoneGroup?> GetGroupWithMembersAsync(int groupId)
    {
        return await _context.CapstoneGroups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == groupId);
    }

    public async Task<CapstoneGroup?> GetGroupWithTopicAsync(int groupId)
    {
        return await _context.CapstoneGroups
            .Include(g => g.Topics)
            .FirstOrDefaultAsync(g => g.Id == groupId);
    }

    public async Task<CapstoneGroup?> GetGroupWithMembersAndTopicAsync(int groupId)
    {
        return await _context.CapstoneGroups
            .Include(g => g.Members)
            .Include(g => g.Topics)
            .FirstOrDefaultAsync(g => g.Id == groupId);
    }

    public async Task<bool> UserHasGroupAsync(string userId)
    {
        return await _context.CapstoneGroups
            .AnyAsync(g => g.Members.Any(m => m.Id == userId));
    }

    public async Task<CapstoneGroup> CreateGroupAsync(CapstoneGroup group)
    {
        _context.CapstoneGroups.Add(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task AddMemberToGroupAsync(string userId, int groupId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.CapstoneGroupId = groupId;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveMemberFromGroupAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.CapstoneGroupId = null;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateGroupAsync(CapstoneGroup group)
    {
        _context.CapstoneGroups.Update(group);
        await _context.SaveChangesAsync();
    }
}
