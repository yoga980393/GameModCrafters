using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameModCrafters.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommissionStatuses",
                columns: table => new
                {
                    CommissionStatusId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionStatuses", x => x.CommissionStatusId);
                });

            migrationBuilder.CreateTable(
                name: "Counters",
                columns: table => new
                {
                    CounterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false),
                    CounterName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counters", x => x.CounterId);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GameName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TagName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BackgroundImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Baned = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "Commissions",
                columns: table => new
                {
                    CommissionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DelegatorId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GameId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CommissionTitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CommissionDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommissionStatusId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDone = table.Column<bool>(type: "bit", nullable: false),
                    Trash = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commissions", x => x.CommissionId);
                    table.ForeignKey(
                        name: "FK_Commissions_CommissionStatuses_CommissionStatusId",
                        column: x => x.CommissionStatusId,
                        principalTable: "CommissionStatuses",
                        principalColumn: "CommissionStatusId");
                    table.ForeignKey(
                        name: "FK_Commissions_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId");
                    table.ForeignKey(
                        name: "FK_Commissions_Users_DelegatorId",
                        column: x => x.DelegatorId,
                        principalTable: "Users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "ContactUsMessages",
                columns: table => new
                {
                    ContactId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    SubmitTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUsMessages", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_ContactUsMessages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "Mods",
                columns: table => new
                {
                    ModId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GameId = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    AuthorId = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    ModName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstallationInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DownloadLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDone = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mods", x => x.ModId);
                    table.ForeignKey(
                        name: "FK_Mods_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId");
                    table.ForeignKey(
                        name: "FK_Mods_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "PrivateMessages",
                columns: table => new
                {
                    MessageId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateMessages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_PrivateMessages_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Email");
                    table.ForeignKey(
                        name: "FK_PrivateMessages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "CommissionTrackings",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CommissionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AddTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionTrackings", x => new { x.UserId, x.CommissionId });
                    table.ForeignKey(
                        name: "FK_CommissionTrackings_Commissions_CommissionId",
                        column: x => x.CommissionId,
                        principalTable: "Commissions",
                        principalColumn: "CommissionId");
                    table.ForeignKey(
                        name: "FK_CommissionTrackings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CommissionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PayerId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PayeeId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TransactionStatus = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Commissions_CommissionId",
                        column: x => x.CommissionId,
                        principalTable: "Commissions",
                        principalColumn: "CommissionId");
                    table.ForeignKey(
                        name: "FK_Transactions_Users_PayeeId",
                        column: x => x.PayeeId,
                        principalTable: "Users",
                        principalColumn: "Email");
                    table.ForeignKey(
                        name: "FK_Transactions_Users_PayerId",
                        column: x => x.PayerId,
                        principalTable: "Users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "Downloadeds",
                columns: table => new
                {
                    DownloadId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ModId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DownloadTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Downloadeds", x => x.DownloadId);
                    table.ForeignKey(
                        name: "FK_Downloadeds_Mods_ModId",
                        column: x => x.ModId,
                        principalTable: "Mods",
                        principalColumn: "ModId");
                    table.ForeignKey(
                        name: "FK_Downloadeds_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ModId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AddTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => new { x.UserId, x.ModId });
                    table.ForeignKey(
                        name: "FK_Favorites_Mods_ModId",
                        column: x => x.ModId,
                        principalTable: "Mods",
                        principalColumn: "ModId");
                    table.ForeignKey(
                        name: "FK_Favorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ModId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AddTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => new { x.UserId, x.ModId });
                    table.ForeignKey(
                        name: "FK_Logs_Mods_ModId",
                        column: x => x.ModId,
                        principalTable: "Mods",
                        principalColumn: "ModId");
                    table.ForeignKey(
                        name: "FK_Logs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "ModComments",
                columns: table => new
                {
                    CommentId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ModId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CommentContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModComments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_ModComments_Mods_ModId",
                        column: x => x.ModId,
                        principalTable: "Mods",
                        principalColumn: "ModId");
                    table.ForeignKey(
                        name: "FK_ModComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "ModLikes",
                columns: table => new
                {
                    ModId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Liked = table.Column<bool>(type: "bit", nullable: false),
                    RatingDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModLikes", x => new { x.UserId, x.ModId });
                    table.ForeignKey(
                        name: "FK_ModLikes_Mods_ModId",
                        column: x => x.ModId,
                        principalTable: "Mods",
                        principalColumn: "ModId");
                    table.ForeignKey(
                        name: "FK_ModLikes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "ModTags",
                columns: table => new
                {
                    ModId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TagId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModTags", x => new { x.TagId, x.ModId });
                    table.ForeignKey(
                        name: "FK_ModTags_Mods_ModId",
                        column: x => x.ModId,
                        principalTable: "Mods",
                        principalColumn: "ModId");
                    table.ForeignKey(
                        name: "FK_ModTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId");
                });

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

            migrationBuilder.InsertData(
                table: "Counters",
                columns: new[] { "CounterId", "CounterName", "Value" },
                values: new object[,]
                {
                    { 1, "Mod", 0 },
                    { 2, "Commission", 0 }
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "GameId", "CreateTime", "Description", "GameName", "Thumbnail" },
                values: new object[,]
                {
                    { "g001", new DateTime(2023, 5, 27, 17, 21, 0, 0, DateTimeKind.Unspecified), "mcTest", "Minecraft", "mcImg" },
                    { "g002", new DateTime(2023, 5, 28, 10, 30, 0, 0, DateTimeKind.Unspecified), "fnTest", "Fortnite", "fnImg" },
                    { "g003", new DateTime(2023, 5, 29, 14, 15, 0, 0, DateTimeKind.Unspecified), "owTest", "Overwatch", "owImg" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Email", "Avatar", "BackgroundImage", "Baned", "IsAdmin", "LastLogin", "Password", "RegistrationDate", "Username" },
                values: new object[,]
                {
                    { "kevinxi@gmail.com", null, null, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "c824feab835d82155c58d309594283703916ce3d57e14d219d160253c8e0bf2c55ef41e528119077053a67ac7b44dc61781d8d4b1ea447d472c964e49739ca21", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "大明" },
                    { "marylee@gmail.com", null, null, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "86a873d8f1ac8e07b059dc8f9175df802f6949d1d76533f9baf2e482a4e07f41f47e3665e31497351dd68dcceade5855e1c00af490e58d4ee34bd0c8227b921f", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "中明" },
                    { "johnwei@gmail.com", null, null, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "012a24e0b0602f251736b29b8f07304b0e89d6c2ce379e64835973cd11e1ff3d0c8bbb4683bca46a8c7e19dc77a3a0038abf17bc9f2bfd87134306b34eb81c09", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "wTestw" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_CommissionStatusId",
                table: "Commissions",
                column: "CommissionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_DelegatorId",
                table: "Commissions",
                column: "DelegatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_GameId",
                table: "Commissions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionTrackings_CommissionId",
                table: "CommissionTrackings",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactUsMessages_UserId",
                table: "ContactUsMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Downloadeds_ModId",
                table: "Downloadeds",
                column: "ModId");

            migrationBuilder.CreateIndex(
                name: "IX_Downloadeds_UserId",
                table: "Downloadeds",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_ModId",
                table: "Favorites",
                column: "ModId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ModId",
                table: "Logs",
                column: "ModId");

            migrationBuilder.CreateIndex(
                name: "IX_ModComments_ModId",
                table: "ModComments",
                column: "ModId");

            migrationBuilder.CreateIndex(
                name: "IX_ModComments_UserId",
                table: "ModComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModLikes_ModId",
                table: "ModLikes",
                column: "ModId");

            migrationBuilder.CreateIndex(
                name: "IX_Mods_AuthorId",
                table: "Mods",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Mods_GameId",
                table: "Mods",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_ModTags_ModId",
                table: "ModTags",
                column: "ModId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_ReceiverId",
                table: "PrivateMessages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_SenderId",
                table: "PrivateMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CommissionId",
                table: "Transactions",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PayeeId",
                table: "Transactions",
                column: "PayeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PayerId",
                table: "Transactions",
                column: "PayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommissionTrackings");

            migrationBuilder.DropTable(
                name: "ContactUsMessages");

            migrationBuilder.DropTable(
                name: "Counters");

            migrationBuilder.DropTable(
                name: "Downloadeds");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "ModComments");

            migrationBuilder.DropTable(
                name: "ModLikes");

            migrationBuilder.DropTable(
                name: "ModTags");

            migrationBuilder.DropTable(
                name: "PrivateMessages");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Mods");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Commissions");

            migrationBuilder.DropTable(
                name: "CommissionStatuses");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
