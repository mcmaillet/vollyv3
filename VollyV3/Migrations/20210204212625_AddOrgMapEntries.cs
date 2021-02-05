using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VollyV3.Migrations
{
    public partial class AddOrgMapEntries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationMapEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    ContactEmail = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    WebsiteLink = table.Column<string>(nullable: true),
                    MissionStatement = table.Column<string>(nullable: true),
                    FullDescription = table.Column<string>(nullable: true),
                    CauseId = table.Column<int>(nullable: true),
                    LocationId = table.Column<int>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationMapEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationMapEntries_Causes_CauseId",
                        column: x => x.CauseId,
                        principalTable: "Causes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationMapEntries_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMapEntries_CauseId",
                table: "OrganizationMapEntries",
                column: "CauseId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMapEntries_LocationId",
                table: "OrganizationMapEntries",
                column: "LocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationMapEntries");
        }
    }
}
