using CapstoneReviewTool.Entities;

namespace CapstoneReviewTool.ViewModels;

public class ProgressStepViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Detail { get; set; }
    public string? DateNote { get; set; }
    public bool IsMuted { get; set; }
}

public class StudentDashboardViewModel
{
    public CapstoneGroup? Group { get; set; }
    public string TopicStatusDisplay { get; set; } = "Chưa đăng ký";
    public int RegisteredSlotsCount { get; set; }
    public string DefenseScheduleSummary { get; set; } = "Chưa xác định";
    public List<ProgressStepViewModel> ProgressSteps { get; set; } = new();
}
