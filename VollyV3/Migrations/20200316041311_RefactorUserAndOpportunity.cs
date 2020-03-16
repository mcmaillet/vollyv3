using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VollyV3.Migrations
{
    public partial class RefactorUserAndOpportunity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_Categories_CategoryId",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_CategoryId",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Opportunities");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Opportunities",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Opportunities");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Opportunities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_CategoryId",
                table: "Opportunities",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_Categories_CategoryId",
                table: "Opportunities",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
