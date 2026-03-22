using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneReviewTool.Entities
{
    public class DefenseSlotRegistration
    {
        [Key]
        public int Id { get; set; }

        public int DefenseSlotId { get; set; }
        [ForeignKey("DefenseSlotId")]
        public virtual DefenseSlot? DefenseSlot { get; set; }

        public int CapstoneGroupId { get; set; }
        [ForeignKey("CapstoneGroupId")]
        public virtual CapstoneGroup? CapstoneGroup { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Accepted, Rejected, Cancelled

        // Lý do hủy (ví dụ: "Auto cancel because another council approved")
        public string? CancelReason { get; set; }

        public int Priority { get; set; } = 1;
    }
}
