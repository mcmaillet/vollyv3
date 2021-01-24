using Microsoft.EntityFrameworkCore.Migrations;

namespace VollyV3.Migrations
{
    public partial class UpdateVolunteerHoursWithNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerHours_Opportunities_OpportunityId",
                table: "VolunteerHours");

            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerHours_Organizations_OrganizationId",
                table: "VolunteerHours");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerHours_OpportunityId",
                table: "VolunteerHours");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerHours_OrganizationId",
                table: "VolunteerHours");

            migrationBuilder.AddColumn<string>(
                name: "OpportunityName",
                table: "VolunteerHours",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationName",
                table: "VolunteerHours",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpportunityName",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "OrganizationName",
                table: "VolunteerHours");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerHours_OpportunityId",
                table: "VolunteerHours",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerHours_OrganizationId",
                table: "VolunteerHours",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerHours_Opportunities_OpportunityId",
                table: "VolunteerHours",
                column: "OpportunityId",
                principalTable: "Opportunities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerHours_Organizations_OrganizationId",
                table: "VolunteerHours",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
