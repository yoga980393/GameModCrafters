using Microsoft.EntityFrameworkCore.Migrations;

namespace GameModCrafters.Migrations
{
    public partial class AddCounterName : Migration
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Counters",
                keyColumn: "CounterId",
                keyValue: 2);

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
