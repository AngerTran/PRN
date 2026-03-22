using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneReviewTool.Entities
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        
        public string Field { get; set; } = string.Empty; // e.g. "Software Engineering"

        public string? ProposalFileUri { get; set; }
        
        public string? RepoLink { get; set; }

        public string Status { get; set; } = "Draft"; // Draft, Submitted, Approved, Rejected, Revision

        public DateTime? SubmittedDate { get; set; }

        // One Group has One Topic
        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual CapstoneGroup? Group { get; set; }
    }
}
