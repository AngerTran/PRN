using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapstoneReviewTool.Migrations
{
    /// <inheritdoc />
    public partial class AddCommitteeAndRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapstoneGroups_DefenseSlots_DefenseSlotId",
                table: "CapstoneGroups");

            migrationBuilder.DropIndex(
                name: "IX_CapstoneGroups_DefenseSlotId",
                table: "CapstoneGroups");

            migrationBuilder.DropColumn(
                name: "CommitteeName",
                table: "DefenseSlots");

            migrationBuilder.DropColumn(
                name: "DefenseSlotId",
                table: "CapstoneGroups");

            migrationBuilder.AddColumn<int>(
                name: "CommitteeId",
                table: "DefenseSlots",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Committees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Committees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefenseSlotRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DefenseSlotId = table.Column<int>(type: "INTEGER", nullable: false),
                    CapstoneGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefenseSlotRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefenseSlotRegistrations_CapstoneGroups_CapstoneGroupId",
                        column: x => x.CapstoneGroupId,
                        principalTable: "CapstoneGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefenseSlotRegistrations_DefenseSlots_DefenseSlotId",
                        column: x => x.DefenseSlotId,
                        principalTable: "DefenseSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommitteeMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommitteeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommitteeMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommitteeMembers_Committees_CommitteeId",
                        column: x => x.CommitteeId,
                        principalTable: "Committees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefenseSlots_CommitteeId",
                table: "DefenseSlots",
                column: "CommitteeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeMembers_CommitteeId",
                table: "CommitteeMembers",
                column: "CommitteeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeMembers_UserId",
                table: "CommitteeMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DefenseSlotRegistrations_CapstoneGroupId",
                table: "DefenseSlotRegistrations",
                column: "CapstoneGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DefenseSlotRegistrations_DefenseSlotId",
                table: "DefenseSlotRegistrations",
                column: "DefenseSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_DefenseSlots_Committees_CommitteeId",
                table: "DefenseSlots",
                column: "CommitteeId",
                principalTable: "Committees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DefenseSlots_Committees_CommitteeId",
                table: "DefenseSlots");

            migrationBuilder.DropTable(
                name: "CommitteeMembers");

            migrationBuilder.DropTable(
                name: "DefenseSlotRegistrations");

            migrationBuilder.DropTable(
                name: "Committees");

            migrationBuilder.DropIndex(
                name: "IX_DefenseSlots_CommitteeId",
                table: "DefenseSlots");

            migrationBuilder.DropColumn(
                name: "CommitteeId",
                table: "DefenseSlots");

            migrationBuilder.AddColumn<string>(
                name: "CommitteeName",
                table: "DefenseSlots",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefenseSlotId",
                table: "CapstoneGroups",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CapstoneGroups_DefenseSlotId",
                table: "CapstoneGroups",
                column: "DefenseSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_CapstoneGroups_DefenseSlots_DefenseSlotId",
                table: "CapstoneGroups",
                column: "DefenseSlotId",
                principalTable: "DefenseSlots",
                principalColumn: "Id");
        }
    }
}
