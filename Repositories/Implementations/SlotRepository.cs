using Microsoft.EntityFrameworkCore;
using CapstoneReviewTool.Data;
using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Repositories.Interfaces;

namespace CapstoneReviewTool.Repositories.Implementations;

public class SlotRepository : ISlotRepository
{
    private readonly ApplicationDbContext _context;

    public SlotRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DefenseSlot>> GetAllSlotsWithDetailsAsync()
    {
        return await _context.DefenseSlots
            .Include(s => s.Registrations)
                .ThenInclude(r => r.CapstoneGroup)
            .Include(s => s.Committee)
                .ThenInclude(c => c.Members)
                    .ThenInclude(m => m.User)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<DefenseSlot?> GetSlotByIdAsync(int slotId)
    {
        return await _context.DefenseSlots
            .Include(s => s.Registrations)
            .FirstOrDefaultAsync(s => s.Id == slotId);
    }

    public async Task<DefenseSlot?> GetSlotWithRegistrationsAsync(int slotId)
    {
        return await _context.DefenseSlots
            .Include(s => s.Registrations)
                .ThenInclude(r => r.CapstoneGroup)
                    .ThenInclude(g => g.Leader)
            .Include(s => s.Registrations)
                .ThenInclude(r => r.CapstoneGroup)
                    .ThenInclude(g => g.Topics)
            .Include(s => s.Committee)
            .FirstOrDefaultAsync(s => s.Id == slotId);
    }

    public async Task<DefenseSlot> CreateSlotAsync(DefenseSlot slot)
    {
        _context.DefenseSlots.Add(slot);
        await _context.SaveChangesAsync();
        return slot;
    }

    public async Task UpdateSlotAsync(DefenseSlot slot)
    {
        _context.DefenseSlots.Update(slot);
        await _context.SaveChangesAsync();
    }

    public async Task<List<DefenseSlot>> GetSlotsByCommitteeIdAsync(int? committeeId)
    {
        return await _context.DefenseSlots
            .Where(s => s.CommitteeId == committeeId)
            .Include(s => s.Registrations)
            .ToListAsync();
    }
}
