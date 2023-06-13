using Microsoft.EntityFrameworkCore.Migrations;

namespace GameModCrafters.Migrations
{
    public partial class InitialDB124 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "emailconfirmed",
                table: "Users",
                newName: "EmailConfirmed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailConfirmed",
                table: "Users",
                newName: "emailconfirmed");
        }
    }
}
