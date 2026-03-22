using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneReviewTool.Entities
{
    public class CommitteeMember
    {
        [Key]
        public int Id { get; set; }

        public int CommitteeId { get; set; }
        [ForeignKey("CommitteeId")]
        public virtual Committee? Committee { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}
