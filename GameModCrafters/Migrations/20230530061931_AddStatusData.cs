using Microsoft.EntityFrameworkCore.Migrations;

namespace GameModCrafters.Migrations
{
    public partial class AddStatusData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CommissionStatuses",
                columns: new[] { "CommissionStatusId", "Status" },
                values: new object[,]
                {
                    { "s01", "待接受" },
                    { "s02", "進行中" },
                    { "s03", "已完成" },
                    { "s04", "已取消" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CommissionStatuses",
                keyColumn: "CommissionStatusId",
                keyValue: "s01");

            migrationBuilder.DeleteData(
                table: "CommissionStatuses",
                keyColumn: "CommissionStatusId",
                keyValue: "s02");

            migrationBuilder.DeleteData(
                table: "CommissionStatuses",
                keyColumn: "CommissionStatusId",
                keyValue: "s03");

            migrationBuilder.DeleteData(
                table: "CommissionStatuses",
                keyColumn: "CommissionStatusId",
                keyValue: "s04");
        }
    }
}
