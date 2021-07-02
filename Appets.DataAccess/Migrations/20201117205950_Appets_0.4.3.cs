using Microsoft.EntityFrameworkCore.Migrations;

namespace Appets.DataAccess.Migrations
{
    public partial class Appets_043 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IgnoredPosts",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IgnoredPosts",
                table: "Posts");
        }
    }
}
