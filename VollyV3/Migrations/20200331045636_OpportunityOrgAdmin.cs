using Microsoft.EntityFrameworkCore.Migrations;

namespace VollyV3.Migrations
{
    public partial class OpportunityOrgAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_OrganizationAdministratorUsers_CreatedByUserUserId_CreatedByUserOrganizationId",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_CreatedByUserUserId_CreatedByUserOrganizationId",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "CreatedByUserOrganizationId",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "CreatedByUserUserId",
                table: "Opportunities");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "Opportunities",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByOrganizationId",
                table: "Opportunities",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_CreatedByUserId_CreatedByOrganizationId",
                table: "Opportunities",
                columns: new[] { "CreatedByUserId", "CreatedByOrganizationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_OrganizationAdministratorUsers_CreatedByUserId_CreatedByOrganizationId",
                table: "Opportunities",
                columns: new[] { "CreatedByUserId", "CreatedByOrganizationId" },
                principalTable: "OrganizationAdministratorUsers",
                principalColumns: new[] { "UserId", "OrganizationId" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_OrganizationAdministratorUsers_CreatedByUserId_CreatedByOrganizationId",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_CreatedByUserId_CreatedByOrganizationId",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "CreatedByOrganizationId",
                table: "Opportunities");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "Opportunities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserOrganizationId",
                table: "Opportunities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserUserId",
                table: "Opportunities",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_CreatedByUserUserId_CreatedByUserOrganizationId",
                table: "Opportunities",
                columns: new[] { "CreatedByUserUserId", "CreatedByUserOrganizationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_OrganizationAdministratorUsers_CreatedByUserUserId_CreatedByUserOrganizationId",
                table: "Opportunities",
                columns: new[] { "CreatedByUserUserId", "CreatedByUserOrganizationId" },
                principalTable: "OrganizationAdministratorUsers",
                principalColumns: new[] { "UserId", "OrganizationId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
