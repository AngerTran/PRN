namespace CapstoneReviewTool.ViewModels;

public class ChatMessageViewModel
{
    public int Id { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAtUtc { get; set; }
    public bool IsNotification { get; set; }
}
