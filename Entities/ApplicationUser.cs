using Microsoft.AspNetCore.Identity;

namespace CapstoneReviewTool.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    public string? StudentId { get; set; }
    public string? LecturerCode { get; set; }
    
    // Foreign Key to CapstoneGroup
    public int? CapstoneGroupId { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.ForeignKey("CapstoneGroupId")]
    public virtual CapstoneGroup? CapstoneGroup { get; set; }
}
