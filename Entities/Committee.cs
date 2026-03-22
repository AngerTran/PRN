using System.ComponentModel.DataAnnotations;

namespace CapstoneReviewTool.Entities
{
    public class Committee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<CommitteeMember> Members { get; set; } = new List<CommitteeMember>();
    }
}
