using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameModCrafters.Migrations
{
    public partial class MH : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "GameId",
                keyValue: "g001",
                column: "Thumbnail",
                value: "/GameImages/mcImgmcImg.jpg");

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "GameId",
                keyValue: "g004",
                column: "CreateTime",
                value: new DateTime(2023, 5, 30, 14, 15, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "GameId", "CreateTime", "Description", "GameName", "Thumbnail" },
                values: new object[] { "g005", new DateTime(2023, 5, 31, 14, 15, 0, 0, DateTimeKind.Unspecified), "魔物獵人", "魔物獵人", "/GameImages/MH.jpg" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "GameId",
                keyValue: "g005");

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "GameId",
                keyValue: "g001",
                column: "Thumbnail",
                value: "/GameImages/mcImg.jpg");

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "GameId",
                keyValue: "g004",
                column: "CreateTime",
                value: new DateTime(2023, 5, 29, 14, 15, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
