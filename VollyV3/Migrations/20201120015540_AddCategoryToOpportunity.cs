using Microsoft.EntityFrameworkCore.Migrations;

namespace VollyV3.Migrations
{
    public partial class AddCategoryToOpportunity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Opportunities",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
