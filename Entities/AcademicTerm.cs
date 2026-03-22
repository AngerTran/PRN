using System.ComponentModel.DataAnnotations;

namespace CapstoneReviewTool.Entities;

public class AcademicTerm
{
    public int Id { get; set; }

    [Required]
    [StringLength(32)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(128)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}
