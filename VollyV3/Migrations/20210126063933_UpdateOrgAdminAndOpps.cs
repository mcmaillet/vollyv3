using Microsoft.EntityFrameworkCore.Migrations;

namespace VollyV3.Migrations
{
    public partial class UpdateOrgAdminAndOpps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Opportunities");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Opportunities",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Opportunities",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_CreatedById",
                table: "Opportunities",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_OrganizationId",
                table: "Opportunities",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_AspNetUsers_CreatedById",
                table: "Opportunities",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_Organizations_OrganizationId",
                table: "Opportunities",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_AspNetUsers_CreatedById",
                table: "Opportunities");

            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_Organizations_OrganizationId",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_CreatedById",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_OrganizationId",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Opportunities");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByOrganizationId",
                table: "Opportunities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Opportunities",
                type: "nvarchar(450)",
                nullable: true);

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
    }
}
