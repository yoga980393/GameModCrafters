using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameModCrafters.Migrations
{
    public partial class AddSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ModName",
                table: "Mods",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Counters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Counters",
                keyColumn: "CounterId",
                keyValue: 1,
                column: "Name",
                value: "Mod");

            migrationBuilder.InsertData(
                table: "Counters",
                columns: new[] { "CounterId", "Name", "Value" },
                values: new object[] { 2, "Commission", 0 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Email", "Avatar", "BackgroundImage", "Baned", "IsAdmin", "LastLogin", "Password", "RegistrationDate", "Username" },
                values: new object[,]
                {
                    { "kevinxi@gmail.com", null, null, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "c824feab835d82155c58d309594283703916ce3d57e14d219d160253c8e0bf2c55ef41e528119077053a67ac7b44dc61781d8d4b1ea447d472c964e49739ca21", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "大明" },
                    { "marylee@gmail.com", null, null, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "86a873d8f1ac8e07b059dc8f9175df802f6949d1d76533f9baf2e482a4e07f41f47e3665e31497351dd68dcceade5855e1c00af490e58d4ee34bd0c8227b921f", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "中明" },
                    { "johnwei@gmail.com", null, null, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "012a24e0b0602f251736b29b8f07304b0e89d6c2ce379e64835973cd11e1ff3d0c8bbb4683bca46a8c7e19dc77a3a0038abf17bc9f2bfd87134306b34eb81c09", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "wTestw" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Counters",
                keyColumn: "CounterId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Email",
                keyValue: "johnwei@gmail.com");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Email",
                keyValue: "kevinxi@gmail.com");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Email",
                keyValue: "marylee@gmail.com");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Counters");

            migrationBuilder.AlterColumn<string>(
                name: "ModName",
                table: "Mods",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }
    }
}
