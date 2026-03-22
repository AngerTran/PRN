using CapstoneReviewTool.Entities;

namespace CapstoneReviewTool.Repositories.Interfaces;

public interface IAcademicTermRepository
{
    Task<AcademicTerm?> GetActiveTermAsync();
    Task<AcademicTerm?> GetByIdAsync(int id);
}
