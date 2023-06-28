﻿// <auto-generated />
using System;
using GameModCrafters.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GameModCrafters.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GameModCrafters.Models.Commission", b =>
                {
                    b.Property<string>("CommissionId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<decimal?>("Budget")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("CommissionDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CommissionStatusId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CommissionTitle")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("Deadline")
                        .HasColumnType("datetime2");

                    b.Property<string>("DelegatorId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("GameId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsDone")
                        .HasColumnType("bit");

                    b.Property<bool>("Trash")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("CommissionId");

                    b.HasIndex("CommissionStatusId");

                    b.HasIndex("DelegatorId");

                    b.HasIndex("GameId");

                    b.ToTable("Commissions");
                });

            modelBuilder.Entity("GameModCrafters.Models.CommissionStatus", b =>
                {
                    b.Property<string>("CommissionStatusId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("CommissionStatusId");

                    b.ToTable("CommissionStatuses");

                    b.HasData(
                        new
                        {
                            CommissionStatusId = "s01",
                            Status = "待接受"
                        },
                        new
                        {
                            CommissionStatusId = "s02",
                            Status = "進行中"
                        },
                        new
                        {
                            CommissionStatusId = "s03",
                            Status = "已完成"
                        },
                        new
                        {
                            CommissionStatusId = "s04",
                            Status = "已取消"
                        });
                });

            modelBuilder.Entity("GameModCrafters.Models.CommissionTracking", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CommissionId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("AddTime")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId", "CommissionId");

                    b.HasIndex("CommissionId");

                    b.ToTable("CommissionTrackings");
                });

            modelBuilder.Entity("GameModCrafters.Models.ContactUs", b =>
                {
                    b.Property<string>("ContactId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("SubmitTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("ContactId");

                    b.HasIndex("UserId");

                    b.ToTable("ContactUsMessages");
                });

            modelBuilder.Entity("GameModCrafters.Models.Counter", b =>
                {
                    b.Property<int>("CounterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CounterName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("CounterId");

                    b.ToTable("Counters");

                    b.HasData(
                        new
                        {
                            CounterId = 1,
                            CounterName = "Mod",
                            Value = 0
                        },
                        new
                        {
                            CounterId = 2,
                            CounterName = "Commission",
                            Value = 0
                        });
                });

            modelBuilder.Entity("GameModCrafters.Models.Downloaded", b =>
                {
                    b.Property<string>("DownloadId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("DownloadTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("DownloadId");

                    b.HasIndex("ModId");

                    b.HasIndex("UserId");

                    b.ToTable("Downloadeds");
                });

            modelBuilder.Entity("GameModCrafters.Models.Favorite", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ModId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("AddTime")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId", "ModId");

                    b.HasIndex("ModId");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("GameModCrafters.Models.Game", b =>
                {
                    b.Property<string>("GameId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GameName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Thumbnail")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GameId");

                    b.ToTable("Games");

                    b.HasData(
                        new
                        {
                            GameId = "g001",
                            CreateTime = new DateTime(2023, 5, 27, 17, 21, 0, 0, DateTimeKind.Unspecified),
                            Description = "mcTest",
                            GameName = "Minecraft",
                            Thumbnail = "/GameImages/mcImg.jpg"
                        },
                        new
                        {
                            GameId = "g002",
                            CreateTime = new DateTime(2023, 5, 28, 10, 30, 0, 0, DateTimeKind.Unspecified),
                            Description = "fnTest",
                            GameName = "Fortnite",
                            Thumbnail = "/GameImages/fnImg.jpg"
                        },
                        new
                        {
                            GameId = "g003",
                            CreateTime = new DateTime(2023, 5, 29, 14, 15, 0, 0, DateTimeKind.Unspecified),
                            Description = "owTest",
                            GameName = "Overwatch",
                            Thumbnail = "/GameImages/owImg.jpg"
                        });
                });

            modelBuilder.Entity("GameModCrafters.Models.Log", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ModId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("AddTime")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId", "ModId");

                    b.HasIndex("ModId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("GameModCrafters.Models.Mod", b =>
                {
                    b.Property<string>("ModId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("AuthorId")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DownloadLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GameId")
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("InstallationInstructions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDone")
                        .HasColumnType("bit");

                    b.Property<string>("ModName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Thumbnail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("ModId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("GameId");

                    b.ToTable("Mods");
                });

            modelBuilder.Entity("GameModCrafters.Models.ModComment", b =>
                {
                    b.Property<string>("CommentId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CommentContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CommentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("CommentId");

                    b.HasIndex("ModId");

                    b.HasIndex("UserId");

                    b.ToTable("ModComments");
                });

            modelBuilder.Entity("GameModCrafters.Models.ModLike", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ModId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("Liked")
                        .HasColumnType("bit");

                    b.Property<DateTime>("RatingDate")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId", "ModId");

                    b.HasIndex("ModId");

                    b.ToTable("ModLikes");
                });

            modelBuilder.Entity("GameModCrafters.Models.ModTag", b =>
                {
                    b.Property<string>("TagId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ModId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("TagId", "ModId");

                    b.HasIndex("ModId");

                    b.ToTable("ModTags");
                });

            modelBuilder.Entity("GameModCrafters.Models.PrivateMessage", b =>
                {
                    b.Property<string>("MessageId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("MessageContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("MessageTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReceiverId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("SenderId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("MessageId");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("PrivateMessages");
                });

            modelBuilder.Entity("GameModCrafters.Models.Tag", b =>
                {
                    b.Property<string>("TagId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("TagName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("TagId");

                    b.ToTable("Tags");

                    b.HasData(
                        new
                        {
                            TagId = "t001",
                            TagName = "劇情"
                        },
                        new
                        {
                            TagId = "t002",
                            TagName = "數值"
                        },
                        new
                        {
                            TagId = "t003",
                            TagName = "武器"
                        },
                        new
                        {
                            TagId = "t004",
                            TagName = "道具"
                        },
                        new
                        {
                            TagId = "t005",
                            TagName = "地圖"
                        },
                        new
                        {
                            TagId = "t006",
                            TagName = "音樂"
                        },
                        new
                        {
                            TagId = "t007",
                            TagName = "美術"
                        },
                        new
                        {
                            TagId = "t008",
                            TagName = "程式"
                        },
                        new
                        {
                            TagId = "t009",
                            TagName = "其他"
                        });
                });

            modelBuilder.Entity("GameModCrafters.Models.Transaction", b =>
                {
                    b.Property<string>("TransactionId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CommissionId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("PayeeId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PayerId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("TransactionStatus")
                        .HasColumnType("bit");

                    b.HasKey("TransactionId");

                    b.HasIndex("CommissionId");

                    b.HasIndex("PayeeId");

                    b.HasIndex("PayerId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("GameModCrafters.Models.User", b =>
                {
                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BackgroundImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Baned")
                        .HasColumnType("bit");

                    b.Property<string>("ConfirmationCode")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("datetime2");

                    b.Property<int>("ModCoin")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PayPalAccounts")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Email");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Email = "kevinxi@gmail.com",
                            Baned = false,
                            EmailConfirmed = false,
                            IsAdmin = false,
                            LastLogin = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModCoin = 0,
                            Password = "c824feab835d82155c58d309594283703916ce3d57e14d219d160253c8e0bf2c55ef41e528119077053a67ac7b44dc61781d8d4b1ea447d472c964e49739ca21",
                            RegistrationDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Username = "大明"
                        },
                        new
                        {
                            Email = "marylee@gmail.com",
                            Baned = false,
                            EmailConfirmed = false,
                            IsAdmin = false,
                            LastLogin = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModCoin = 0,
                            Password = "86a873d8f1ac8e07b059dc8f9175df802f6949d1d76533f9baf2e482a4e07f41f47e3665e31497351dd68dcceade5855e1c00af490e58d4ee34bd0c8227b921f",
                            RegistrationDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Username = "中明"
                        },
                        new
                        {
                            Email = "johnwei@gmail.com",
                            Baned = false,
                            EmailConfirmed = false,
                            IsAdmin = false,
                            LastLogin = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModCoin = 0,
                            Password = "012a24e0b0602f251736b29b8f07304b0e89d6c2ce379e64835973cd11e1ff3d0c8bbb4683bca46a8c7e19dc77a3a0038abf17bc9f2bfd87134306b34eb81c09",
                            RegistrationDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Username = "wTestw"
                        });
                });

            modelBuilder.Entity("GameModCrafters.Models.Commission", b =>
                {
                    b.HasOne("GameModCrafters.Models.CommissionStatus", "CommissionStatus")
                        .WithMany("Commissions")
                        .HasForeignKey("CommissionStatusId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("GameModCrafters.Models.User", "Delegator")
                        .WithMany("Commission")
                        .HasForeignKey("DelegatorId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("GameModCrafters.Models.Game", "Game")
                        .WithMany("Commission")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("CommissionStatus");

                    b.Navigation("Delegator");

                    b.Navigation("Game");
                });

            modelBuilder.Entity("GameModCrafters.Models.CommissionTracking", b =>
                {
                    b.HasOne("GameModCrafters.Models.Commission", "Commission")
                        .WithMany("CommissionTrackings")
                        .HasForeignKey("CommissionId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GameModCrafters.Models.User", "User")
                        .WithMany("CommissionTrackings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Commission");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GameModCrafters.Models.ContactUs", b =>
                {
                    b.HasOne("GameModCrafters.Models.User", "User")
                        .WithMany("ContactMessages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("GameModCrafters.Models.Downloaded", b =>
                {
                    b.HasOne("GameModCrafters.Models.Mod", "Mod")
                        .WithMany("Downloaded")
                        .HasForeignKey("ModId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GameModCrafters.Models.User", "User")
                        .WithMany("Downloaded")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Mod");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GameModCrafters.Models.Favorite", b =>
                {
                    b.HasOne("GameModCrafters.Models.Mod", "Mod")
                        .WithMany("Favorite")
                        .HasForeignKey("ModId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GameModCrafters.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Mod");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GameModCrafters.Models.Log", b =>
                {
                    b.HasOne("GameModCrafters.Models.Mod", "Mod")
                        .WithMany("Log")
                        .HasForeignKey("ModId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GameModCrafters.Models.User", "User")
                        .WithMany("Log")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Mod");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GameModCrafters.Models.Mod", b =>
                {
                    b.HasOne("GameModCrafters.Models.User", "Author")
                        .WithMany("Mods")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GameModCrafters.Models.Game", "Game")
                        .WithMany("Mods")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Author");

                    b.Navigation("Game");
                });

            modelBuilder.Entity("GameModCrafters.Models.ModComment", b =>
                {
                    b.HasOne("GameModCrafters.Models.Mod", "Mod")
                        .WithMany("ModComment")
                        .HasForeignKey("ModId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GameModCrafters.Models.User", "User")
                        .WithMany("ModComment")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Mod");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GameModCrafters.Models.ModLike", b =>
                {
                    b.HasOne("GameModCrafters.Models.Mod", "Mod")
                        .WithMany("ModLikes")
                        .HasForeignKey("ModId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GameModCrafters.Models.User", "User")
                        .WithMany("ModLikes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Mod");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GameModCrafters.Models.ModTag", b =>
                {
                    b.HasOne("GameModCrafters.Models.Mod", "Mod")
                        .WithMany("ModTags")
                        .HasForeignKey("ModId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GameModCrafters.Models.Tag", "Tag")
                        .WithMany("ModTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Mod");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("GameModCrafters.Models.PrivateMessage", b =>
                {
                    b.HasOne("GameModCrafters.Models.User", "Receiver")
                        .WithMany("ReceivedMessages")
                        .HasForeignKey("ReceiverId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GameModCrafters.Models.User", "Sender")
                        .WithMany("SentMessages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("GameModCrafters.Models.Transaction", b =>
                {
                    b.HasOne("GameModCrafters.Models.Commission", "Commission")
                        .WithMany("Transaction")
                        .HasForeignKey("CommissionId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GameModCrafters.Models.User", "Payee")
                        .WithMany("Incomes")
                        .HasForeignKey("PayeeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("GameModCrafters.Models.User", "Payer")
                        .WithMany("Payments")
                        .HasForeignKey("PayerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Commission");

                    b.Navigation("Payee");

                    b.Navigation("Payer");
                });

            modelBuilder.Entity("GameModCrafters.Models.Commission", b =>
                {
                    b.Navigation("CommissionTrackings");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("GameModCrafters.Models.CommissionStatus", b =>
                {
                    b.Navigation("Commissions");
                });

            modelBuilder.Entity("GameModCrafters.Models.Game", b =>
                {
                    b.Navigation("Commission");

                    b.Navigation("Mods");
                });

            modelBuilder.Entity("GameModCrafters.Models.Mod", b =>
                {
                    b.Navigation("Downloaded");

                    b.Navigation("Favorite");

                    b.Navigation("Log");

                    b.Navigation("ModComment");

                    b.Navigation("ModLikes");

                    b.Navigation("ModTags");
                });

            modelBuilder.Entity("GameModCrafters.Models.Tag", b =>
                {
                    b.Navigation("ModTags");
                });

            modelBuilder.Entity("GameModCrafters.Models.User", b =>
                {
                    b.Navigation("Commission");

                    b.Navigation("CommissionTrackings");

                    b.Navigation("ContactMessages");

                    b.Navigation("Downloaded");

                    b.Navigation("Incomes");

                    b.Navigation("Log");

                    b.Navigation("ModComment");

                    b.Navigation("ModLikes");

                    b.Navigation("Mods");

                    b.Navigation("Payments");

                    b.Navigation("ReceivedMessages");

                    b.Navigation("SentMessages");
                });
#pragma warning restore 612, 618
        }
    }
}
