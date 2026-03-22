using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapstoneReviewTool.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CapstoneGroupId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DefenseSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Room = table.Column<string>(type: "TEXT", nullable: false),
                    Mode = table.Column<string>(type: "TEXT", nullable: false),
                    MaxGroups = table.Column<int>(type: "INTEGER", nullable: false),
                    CommitteeName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefenseSlots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CapstoneGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LeaderId = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    DefenseSlotId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapstoneGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CapstoneGroups_AspNetUsers_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CapstoneGroups_DefenseSlots_DefenseSlotId",
                        column: x => x.DefenseSlotId,
                        principalTable: "DefenseSlots",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Field = table.Column<string>(type: "TEXT", nullable: false),
                    ProposalFileUri = table.Column<string>(type: "TEXT", nullable: true),
                    RepoLink = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GroupId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topics_CapstoneGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "CapstoneGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CapstoneGroupId",
                table: "AspNetUsers",
                column: "CapstoneGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CapstoneGroups_DefenseSlotId",
                table: "CapstoneGroups",
                column: "DefenseSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_CapstoneGroups_LeaderId",
                table: "CapstoneGroups",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_GroupId",
                table: "Topics",
                column: "GroupId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CapstoneGroups_CapstoneGroupId",
                table: "AspNetUsers",
                column: "CapstoneGroupId",
                principalTable: "CapstoneGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CapstoneGroups_CapstoneGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "CapstoneGroups");

            migrationBuilder.DropTable(
                name: "DefenseSlots");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CapstoneGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CapstoneGroupId",
                table: "AspNetUsers");
        }
    }
}
