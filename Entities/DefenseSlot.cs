using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneReviewTool.Entities
{
    public class DefenseSlot
    {
        [Key]
        public int Id { get; set; }

        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }

        public string Room { get; set; } = string.Empty; // e.g. "Room 201" or "Zoom Link"

        public string Mode { get; set; } = "Offline"; // Online, Offline

        public int MaxGroups { get; set; } = 1;

        // Committee assignment (Optional for now, but good to have)
        public int? CommitteeId { get; set; }
        [ForeignKey("CommitteeId")]
        public virtual Committee? Committee { get; set; }

        public virtual ICollection<DefenseSlotRegistration> Registrations { get; set; } = new List<DefenseSlotRegistration>();
    }
}
