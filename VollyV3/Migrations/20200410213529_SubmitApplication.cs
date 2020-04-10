using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VollyV3.Migrations
{
    public partial class SubmitApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_AspNetUsers_VollyV3UserId",
                table: "Applications");

            migrationBuilder.DropTable(
                name: "OccurrenceApplications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_VollyV3UserId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "VollyV3UserId",
                table: "Applications");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Applications",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OccurrenceId",
                table: "Applications",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedDateTime",
                table: "Applications",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Applications_OccurrenceId",
                table: "Applications",
                column: "OccurrenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_UserId",
                table: "Applications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Occurrences_OccurrenceId",
                table: "Applications",
                column: "OccurrenceId",
                principalTable: "Occurrences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_AspNetUsers_UserId",
                table: "Applications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Occurrences_OccurrenceId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_AspNetUsers_UserId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_OccurrenceId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_UserId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "OccurrenceId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "SubmittedDateTime",
                table: "Applications");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "Applications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "VollyV3UserId",
                table: "Applications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OccurrenceApplications",
                columns: table => new
                {
                    OccurrenceId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_Applications_VollyV3UserId",
                table: "Applications",
                column: "VollyV3UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OccurrenceApplications_ApplicationId",
                table: "OccurrenceApplications",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_AspNetUsers_VollyV3UserId",
                table: "Applications",
                column: "VollyV3UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
