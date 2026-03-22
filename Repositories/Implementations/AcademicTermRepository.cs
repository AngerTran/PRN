using Microsoft.EntityFrameworkCore;
using CapstoneReviewTool.Data;
using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Repositories.Interfaces;

namespace CapstoneReviewTool.Repositories.Implementations;

public class AcademicTermRepository : IAcademicTermRepository
{
    private readonly ApplicationDbContext _context;

    public AcademicTermRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AcademicTerm?> GetActiveTermAsync()
    {
        return await _context.AcademicTerms
            .AsNoTracking()
            .Where(t => t.IsActive)
            .OrderBy(t => t.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<AcademicTerm?> GetByIdAsync(int id)
    {
        return await _context.AcademicTerms.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
    }
}
