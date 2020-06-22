using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VollyV3.Migrations
{
    public partial class AddOrgToHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "VolunteerHours",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "VolunteerHours",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerHours_OrganizationId",
                table: "VolunteerHours",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerHours_Organizations_OrganizationId",
                table: "VolunteerHours",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerHours_Organizations_OrganizationId",
                table: "VolunteerHours");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerHours_OrganizationId",
                table: "VolunteerHours");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "VolunteerHours");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "VolunteerHours",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
