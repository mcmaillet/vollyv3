using Microsoft.EntityFrameworkCore.Migrations;

namespace VollyV3.Migrations
{
    public partial class RemoveCommunities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_Communities_CommunityId",
                table: "Opportunities");

            migrationBuilder.DropTable(
                name: "Communities");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_CommunityId",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "Opportunities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommunityId",
                table: "Opportunities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_CommunityId",
                table: "Opportunities",
                column: "CommunityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_Communities_CommunityId",
                table: "Opportunities",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
