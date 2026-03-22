using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneReviewTool.Entities
{
    public class CapstoneGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Foreign Key to Leader (User)
        public string? LeaderId { get; set; }
        [ForeignKey("LeaderId")]
        public virtual ApplicationUser? Leader { get; set; }

        public string Status { get; set; } = "Created"; // Created, Full, Locked

        // Navigation properties
        public virtual ICollection<ApplicationUser> Members { get; set; } = new List<ApplicationUser>();
        
        public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
    }
}
