using Microsoft.EntityFrameworkCore.Migrations;

namespace GameModCrafters.Migrations
{
    public partial class addGameImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "GameId",
                keyValue: "g002",
                column: "Thumbnail",
                value: "/GameImages/fnImg.jpg");

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "GameId",
                keyValue: "g003",
                column: "Thumbnail",
                value: "/GameImages/owImg.jpg");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "GameId",
                keyValue: "g002",
                column: "Thumbnail",
                value: "fnImg.jpg");

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "GameId",
                keyValue: "g003",
                column: "Thumbnail",
                value: "owImg.jpg");
        }
    }
}
