using Microsoft.EntityFrameworkCore.Migrations;

namespace GameModCrafters.Migrations
{
    public partial class AddCounterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commissions_CommissionStatuses_CommissionStatusId",
                table: "Commissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Commissions_Games_GameId",
                table: "Commissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Commissions_Users_DelegatorId",
                table: "Commissions");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionTrackings_Commissions_CommissionId",
                table: "CommissionTrackings");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionTrackings_Users_UserId",
                table: "CommissionTrackings");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactUsMessages_Users_UserId",
                table: "ContactUsMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Downloadeds_Mods_ModId",
                table: "Downloadeds");

            migrationBuilder.DropForeignKey(
                name: "FK_Downloadeds_Users_UserId",
                table: "Downloadeds");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Mods_ModId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Users_UserId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Mods_ModId",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_UserId",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_ModComments_Mods_ModId",
                table: "ModComments");

            migrationBuilder.DropForeignKey(
                name: "FK_ModComments_Users_UserId",
                table: "ModComments");

            migrationBuilder.DropForeignKey(
                name: "FK_ModLikes_Mods_ModId",
                table: "ModLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_ModLikes_Users_UserId",
                table: "ModLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Mods_Games_GameId",
                table: "Mods");

            migrationBuilder.DropForeignKey(
                name: "FK_Mods_Users_AuthorId",
                table: "Mods");

            migrationBuilder.DropForeignKey(
                name: "FK_ModTags_Mods_ModId",
                table: "ModTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ModTags_Tags_TagId",
                table: "ModTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Commissions_CommissionId",
                table: "Transactions");

            migrationBuilder.CreateTable(
                name: "Counters",
                columns: table => new
                {
                    CounterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counters", x => x.CounterId);
                });

            migrationBuilder.InsertData(
                table: "Counters",
                columns: new[] { "CounterId", "Value" },
                values: new object[] { 1, 0 });

            migrationBuilder.AddForeignKey(
                name: "FK_Commissions_CommissionStatuses_CommissionStatusId",
                table: "Commissions",
                column: "CommissionStatusId",
                principalTable: "CommissionStatuses",
                principalColumn: "CommissionStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Commissions_Games_GameId",
                table: "Commissions",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Commissions_Users_DelegatorId",
                table: "Commissions",
                column: "DelegatorId",
                principalTable: "Users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionTrackings_Commissions_CommissionId",
                table: "CommissionTrackings",
                column: "CommissionId",
                principalTable: "Commissions",
                principalColumn: "CommissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionTrackings_Users_UserId",
                table: "CommissionTrackings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactUsMessages_Users_UserId",
                table: "ContactUsMessages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_Downloadeds_Mods_ModId",
                table: "Downloadeds",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "ModId");

            migrationBuilder.AddForeignKey(
                name: "FK_Downloadeds_Users_UserId",
                table: "Downloadeds",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Mods_ModId",
                table: "Favorites",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "ModId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Users_UserId",
                table: "Favorites",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Mods_ModId",
                table: "Logs",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "ModId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_UserId",
                table: "Logs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_ModComments_Mods_ModId",
                table: "ModComments",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "ModId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModComments_Users_UserId",
                table: "ModComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_ModLikes_Mods_ModId",
                table: "ModLikes",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "ModId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModLikes_Users_UserId",
                table: "ModLikes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_Mods_Games_GameId",
                table: "Mods",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mods_Users_AuthorId",
                table: "Mods",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_ModTags_Mods_ModId",
                table: "ModTags",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "ModId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModTags_Tags_TagId",
                table: "ModTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Commissions_CommissionId",
                table: "Transactions",
                column: "CommissionId",
                principalTable: "Commissions",
                principalColumn: "CommissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commissions_CommissionStatuses_CommissionStatusId",
                table: "Commissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Commissions_Games_GameId",
                table: "Commissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Commissions_Users_DelegatorId",
                table: "Commissions");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionTrackings_Commissions_CommissionId",
                table: "CommissionTrackings");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionTrackings_Users_UserId",
                table: "CommissionTrackings");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactUsMessages_Users_UserId",
                table: "ContactUsMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Downloadeds_Mods_ModId",
                table: "Downloadeds");

            migrationBuilder.DropForeignKey(
                name: "FK_Downloadeds_Users_UserId",
                table: "Downloadeds");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Mods_ModId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Users_UserId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Mods_ModId",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_UserId",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_ModComments_Mods_ModId",
                table: "ModComments");

            migrationBuilder.DropForeignKey(
                name: "FK_ModComments_Users_UserId",
                table: "ModComments");

            migrationBuilder.DropForeignKey(
                name: "FK_ModLikes_Mods_ModId",
                table: "ModLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_ModLikes_Users_UserId",
                table: "ModLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Mods_Games_GameId",
                table: "Mods");

            migrationBuilder.DropForeignKey(
                name: "FK_Mods_Users_AuthorId",
                table: "Mods");

            migrationBuilder.DropForeignKey(
                name: "FK_ModTags_Mods_ModId",
                table: "ModTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ModTags_Tags_TagId",
                table: "ModTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Commissions_CommissionId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Counters");

            migrationBuilder.AddForeignKey(
                name: "FK_Commissions_CommissionStatuses_CommissionStatusId",
                table: "Commissions",
                column: "CommissionStatusId",
                principalTable: "CommissionStatuses",
                principalColumn: "CommissionStatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Commissions_Games_GameId",
                table: "Commissions",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Commissions_Users_DelegatorId",
                table: "Commissions",
                column: "DelegatorId",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionTrackings_Commissions_CommissionId",
                table: "CommissionTrackings",
                column: "CommissionId",
                principalTable: "Commissions",
                principalColumn: "CommissionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionTrackings_Users_UserId",
                table: "CommissionTrackings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactUsMessages_Users_UserId",
                table: "ContactUsMessages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Downloadeds_Mods_ModId",
                table: "Downloadeds",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "ModId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Downloadeds_Users_UserId",
                table: "Downloadeds",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Mods_ModId",
                table: "Favorites",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "ModId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Users_UserId",
                table: "Favorites",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Mods_ModId",
                table: "Logs",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "ModId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_UserId",
                table: "Logs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModComments_Mods_ModId",
                table: "ModComments",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "ModId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModComments_Users_UserId",
                table: "ModComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModLikes_Mods_ModId",
                table: "ModLikes",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "ModId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModLikes_Users_UserId",
                table: "ModLikes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mods_Games_GameId",
                table: "Mods",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mods_Users_AuthorId",
                table: "Mods",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModTags_Mods_ModId",
                table: "ModTags",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "ModId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModTags_Tags_TagId",
                table: "ModTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Commissions_CommissionId",
                table: "Transactions",
                column: "CommissionId",
                principalTable: "Commissions",
                principalColumn: "CommissionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
