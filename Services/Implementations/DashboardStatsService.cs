using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CapstoneReviewTool.Data;
using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Services.Interfaces;
using CapstoneReviewTool.ViewModels;

namespace CapstoneReviewTool.Services.Implementations;

public class DashboardStatsService : IDashboardStatsService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardStatsService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<SystemOverviewViewModel> GetSystemOverviewAsync(CancellationToken cancellationToken = default)
    {
        var term = await _context.AcademicTerms
            .AsNoTracking()
            .Where(t => t.IsActive)
            .OrderBy(t => t.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var students = await _userManager.GetUsersInRoleAsync("Student");

        var totalGroups = await _context.CapstoneGroups.AsNoTracking().CountAsync(cancellationToken);
        var totalTopics = await _context.Topics.AsNoTracking().CountAsync(cancellationToken);
        var approvedTopics = await _context.Topics.AsNoTracking()
            .CountAsync(t => t.Status == "Approved", cancellationToken);
        var pendingReviewTopics = await _context.Topics.AsNoTracking()
            .CountAsync(t => t.Status == "Submitted" || t.Status == "Revision", cancellationToken);

        var committeeCount = await _context.Committees.AsNoTracking().CountAsync(cancellationToken);
        var slotCount = await _context.DefenseSlots.AsNoTracking().CountAsync(cancellationToken);

        var slots = await _context.DefenseSlots
            .AsNoTracking()
            .Include(s => s.Registrations)
            .ToListAsync(cancellationToken);

        var slotsWithVacancy = slots.Count(s =>
            s.Registrations.Count(r => r.Status != "Cancelled") < s.MaxGroups);

        var pendingRegs = await _context.DefenseSlotRegistrations.AsNoTracking()
            .CountAsync(r => r.Status == "Pending", cancellationToken);
        var acceptedRegs = await _context.DefenseSlotRegistrations.AsNoTracking()
            .CountAsync(r => r.Status == "Accepted", cancellationToken);

        return new SystemOverviewViewModel
        {
            ActiveTermName = term?.Name,
            ActiveTermCode = term?.Code,
            TotalGroups = totalGroups,
            TotalStudents = students.Count,
            TotalTopics = totalTopics,
            ApprovedTopics = approvedTopics,
            PendingReviewTopics = pendingReviewTopics,
            CommitteeCount = committeeCount,
            DefenseSlotCount = slotCount,
            SlotsWithAvailableSeats = slotsWithVacancy,
            PendingRegistrations = pendingRegs,
            AcceptedRegistrations = acceptedRegs
        };
    }
}
