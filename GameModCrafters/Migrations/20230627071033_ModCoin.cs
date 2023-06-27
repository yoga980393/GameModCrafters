using Microsoft.EntityFrameworkCore.Migrations;

namespace GameModCrafters.Migrations
{
    public partial class ModCoin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModCoin",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModCoin",
                table: "Users");
        }
    }
}
