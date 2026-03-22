using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CapstoneReviewTool.Services.Interfaces;

namespace CapstoneReviewTool.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly IDashboardStatsService _dashboardStats;

    public AdminController(IDashboardStatsService dashboardStats)
    {
        _dashboardStats = dashboardStats;
    }

    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        var stats = await _dashboardStats.GetSystemOverviewAsync(cancellationToken);
        return View(stats);
    }
}
