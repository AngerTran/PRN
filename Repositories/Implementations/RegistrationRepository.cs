using Microsoft.EntityFrameworkCore;
using CapstoneReviewTool.Data;
using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Repositories.Interfaces;

namespace CapstoneReviewTool.Repositories.Implementations;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly ApplicationDbContext _context;

    public RegistrationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DefenseSlotRegistration?> GetRegistrationByIdAsync(int registrationId)
    {
        return await _context.DefenseSlotRegistrations
            .FirstOrDefaultAsync(r => r.Id == registrationId);
    }

    public async Task<List<DefenseSlotRegistration>> GetPendingRegistrationsByGroupAsync(int groupId)
    {
        return await _context.DefenseSlotRegistrations
            .Where(r => r.CapstoneGroupId == groupId && r.Status == "Pending")
            .ToListAsync();
    }

    public async Task<List<DefenseSlotRegistration>> GetActiveRegistrationsByGroupAsync(int groupId)
    {
        return await _context.DefenseSlotRegistrations
            .Where(r => r.CapstoneGroupId == groupId && r.Status != "Cancelled")
            .Include(r => r.DefenseSlot)
            .ToListAsync();
    }

    public async Task<bool> GroupHasAcceptedRegistrationAsync(int groupId)
    {
        return await _context.DefenseSlotRegistrations
            .AnyAsync(r => r.CapstoneGroupId == groupId && r.Status == "Accepted");
    }

    public async Task<DefenseSlotRegistration> CreateRegistrationAsync(DefenseSlotRegistration registration)
    {
        _context.DefenseSlotRegistrations.Add(registration);
        await _context.SaveChangesAsync();
        return registration;
    }

    public async Task UpdateRegistrationAsync(DefenseSlotRegistration registration)
    {
        _context.DefenseSlotRegistrations.Update(registration);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRegistrationAsync(DefenseSlotRegistration registration)
    {
        _context.DefenseSlotRegistrations.Remove(registration);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountActiveRegistrationsByGroupAsync(int groupId)
    {
        return await _context.DefenseSlotRegistrations
            .CountAsync(r => r.CapstoneGroupId == groupId && r.Status != "Cancelled");
    }

    public async Task<bool> GroupHasRegistrationForSlotAsync(int groupId, int slotId)
    {
        return await _context.DefenseSlotRegistrations
            .AnyAsync(r => r.CapstoneGroupId == groupId 
                && r.DefenseSlotId == slotId 
                && r.Status != "Cancelled");
    }
}
