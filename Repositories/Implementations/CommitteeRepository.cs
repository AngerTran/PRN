using Microsoft.EntityFrameworkCore;
using CapstoneReviewTool.Data;
using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Repositories.Interfaces;

namespace CapstoneReviewTool.Repositories.Implementations;

public class CommitteeRepository : ICommitteeRepository
{
    private readonly ApplicationDbContext _context;

    public CommitteeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Committee>> GetAllCommitteesWithMembersAsync()
    {
        return await _context.Committees
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .ToListAsync();
    }

    public async Task<Committee?> GetCommitteeByIdAsync(int committeeId)
    {
        return await _context.Committees
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(c => c.Id == committeeId);
    }

    public async Task<Committee> CreateCommitteeAsync(Committee committee)
    {
        _context.Committees.Add(committee);
        await _context.SaveChangesAsync();
        return committee;
    }

    public async Task AddMemberToCommitteeAsync(int committeeId, string userId)
    {
        var member = new CommitteeMember
        {
            CommitteeId = committeeId,
            UserId = userId
        };
        _context.CommitteeMembers.Add(member);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
}
