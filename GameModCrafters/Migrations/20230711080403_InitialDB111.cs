using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameModCrafters.Migrations
{
    public partial class InitialDB111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "GameId", "CreateTime", "Description", "GameName", "Thumbnail" },
                values: new object[] { "g004", new DateTime(2023, 5, 29, 14, 15, 0, 0, DateTimeKind.Unspecified), "EldenRing", "艾爾登法環", "/GameImages/eldenring.jpg" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "GameId",
                keyValue: "g004");
        }
    }
}
