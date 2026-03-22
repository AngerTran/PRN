using CapstoneReviewTool.ViewModels;

namespace CapstoneReviewTool.Services.Interfaces;

public interface IDashboardStatsService
{
    Task<SystemOverviewViewModel> GetSystemOverviewAsync(CancellationToken cancellationToken = default);
}
