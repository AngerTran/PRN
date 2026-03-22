using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapstoneReviewTool.Migrations
{
    /// <inheritdoc />
    public partial class AddCancelReasonToDefenseSlotRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "DefenseSlotRegistrations",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "DefenseSlotRegistrations");
        }
    }
}
