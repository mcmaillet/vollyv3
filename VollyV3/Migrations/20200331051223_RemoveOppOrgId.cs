using Microsoft.EntityFrameworkCore.Migrations;

namespace VollyV3.Migrations
{
    public partial class RemoveOppOrgId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_Organizations_OrganizationId",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_OrganizationId",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Opportunities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Opportunities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_OrganizationId",
                table: "Opportunities",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_Organizations_OrganizationId",
                table: "Opportunities",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
