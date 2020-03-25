using Microsoft.EntityFrameworkCore.Migrations;

namespace VollyV3.Migrations
{
    public partial class UpdateOccurTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationsOccurrence");

            migrationBuilder.CreateTable(
                name: "OccurrenceApplications",
                columns: table => new
                {
                    OccurrenceId = table.Column<int>(nullable: false),
                    ApplicationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OccurrenceApplications", x => new { x.OccurrenceId, x.ApplicationId });
                    table.ForeignKey(
                        name: "FK_OccurrenceApplications_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OccurrenceApplications_Occurrences_OccurrenceId",
                        column: x => x.OccurrenceId,
                        principalTable: "Occurrences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OccurrenceApplications_ApplicationId",
                table: "OccurrenceApplications",
                column: "ApplicationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OccurrenceApplications");

            migrationBuilder.CreateTable(
                name: "ApplicationsOccurrence",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    OccurrenceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationsOccurrence", x => new { x.ApplicationId, x.OccurrenceId });
                    table.ForeignKey(
                        name: "FK_ApplicationsOccurrence_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationsOccurrence_Occurrences_OccurrenceId",
                        column: x => x.OccurrenceId,
                        principalTable: "Occurrences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationsOccurrence_OccurrenceId",
                table: "ApplicationsOccurrence",
                column: "OccurrenceId");
        }
    }
}
