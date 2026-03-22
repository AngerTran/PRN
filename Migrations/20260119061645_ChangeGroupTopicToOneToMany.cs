using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapstoneReviewTool.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGroupTopicToOneToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Topics_GroupId",
                table: "Topics");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_GroupId",
                table: "Topics",
                column: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Topics_GroupId",
                table: "Topics");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_GroupId",
                table: "Topics",
                column: "GroupId",
                unique: true);
        }
    }
}
