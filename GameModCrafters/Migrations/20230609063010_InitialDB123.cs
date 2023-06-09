using Microsoft.EntityFrameworkCore.Migrations;

namespace GameModCrafters.Migrations
{
    public partial class InitialDB123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "emailconfirmed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "emailconfirmed",
                table: "Users");
        }
    }
}
