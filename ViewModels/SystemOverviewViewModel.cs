namespace CapstoneReviewTool.ViewModels;

/// <summary>
/// Số liệu tổng quan hệ thống — dùng chung Admin / Hội đồng.
/// </summary>
public class SystemOverviewViewModel
{
    public string? ActiveTermName { get; set; }
    public string? ActiveTermCode { get; set; }

    public int TotalGroups { get; set; }
    public int TotalStudents { get; set; }
    public int TotalTopics { get; set; }
    public int ApprovedTopics { get; set; }
    public int PendingReviewTopics { get; set; }

    public int CommitteeCount { get; set; }
    public int DefenseSlotCount { get; set; }
    public int SlotsWithAvailableSeats { get; set; }

    public int PendingRegistrations { get; set; }
    public int AcceptedRegistrations { get; set; }
}
