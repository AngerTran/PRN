using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneReviewTool.Entities;

public class ChatMessage
{
    public int Id { get; set; }

    [Required]
    public string SenderId { get; set; } = string.Empty;

    [ForeignKey("SenderId")]
    public virtual ApplicationUser? Sender { get; set; }

    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;

    public DateTime SentAtUtc { get; set; }

    public ChatChannelType ChannelType { get; set; }

    public int AcademicTermId { get; set; }
    [ForeignKey("AcademicTermId")]
    public virtual AcademicTerm? AcademicTerm { get; set; }

    public int? CapstoneGroupId { get; set; }
    [ForeignKey("CapstoneGroupId")]
    public virtual CapstoneGroup? CapstoneGroup { get; set; }
}
