using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CapstoneReviewTool.Entities;

namespace CapstoneReviewTool.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CapstoneGroup> CapstoneGroups { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<DefenseSlot> DefenseSlots { get; set; }
    public DbSet<Committee> Committees { get; set; }
    public DbSet<CommitteeMember> CommitteeMembers { get; set; }
    public DbSet<DefenseSlotRegistration> DefenseSlotRegistrations { get; set; }
    public DbSet<AcademicTerm> AcademicTerms { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Group - Member relationship
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.CapstoneGroup)
            .WithMany(g => g.Members)
            .HasForeignKey(u => u.CapstoneGroupId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Group - Leader relationship
        builder.Entity<CapstoneGroup>()
            .HasOne(g => g.Leader)
            .WithMany()
            .HasForeignKey(g => g.LeaderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Group - Topic One-to-Many
        builder.Entity<Topic>()
            .HasOne(t => t.Group)
            .WithMany(g => g.Topics)
            .HasForeignKey(t => t.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ChatMessage>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ChatMessage>()
            .HasOne(m => m.AcademicTerm)
            .WithMany(t => t.ChatMessages)
            .HasForeignKey(m => m.AcademicTermId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ChatMessage>()
            .HasOne(m => m.CapstoneGroup)
            .WithMany()
            .HasForeignKey(m => m.CapstoneGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ChatMessage>()
            .HasIndex(m => new { m.AcademicTermId, m.ChannelType, m.CapstoneGroupId, m.SentAtUtc });
    }
}
