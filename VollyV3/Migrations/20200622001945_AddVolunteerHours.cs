using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VollyV3.Migrations
{
    public partial class AddVolunteerHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VolunteerHours",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    OpportunityId = table.Column<int>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Hours = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VolunteerHours_Opportunities_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VolunteerHours_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerHours_OpportunityId",
                table: "VolunteerHours",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerHours_UserId",
                table: "VolunteerHours",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VolunteerHours");
        }
    }
}
