using Microsoft.EntityFrameworkCore;
using GameModCrafters.Models;
using GameModCrafters.Encryption;
using System;

namespace GameModCrafters.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHashService _hashService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,IHashService hashService)
            : base(options)
        {
            _hashService = hashService;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Mod> Mods { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ModTag> ModTags { get; set; }
        public DbSet<ModLike> ModLikes { get; set; }
        public DbSet<ModComment> ModComments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Downloaded> Downloadeds { get; set; }
        public DbSet<CommissionStatus> CommissionStatuses { get; set; }
        public DbSet<Commission> Commissions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }
        public DbSet<ContactUs> ContactUsMessages { get; set; }
        public DbSet<CommissionTracking> CommissionTrackings { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<PurchasedMod> PurchasedMods { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<News> News { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Password以SHA512加密11111
            modelBuilder.Entity<User>().HasData(
                new User {  Username = "大明", Email = "kevinxi@gmail.com", Password = _hashService.SHA512Hash("A12345") },
                new User { Username = "中明", Email = "marylee@gmail.com", Password = _hashService.SHA512Hash("B12345") },
                new User { Username = "wTestw", Email = "johnwei@gmail.com", Password = _hashService.SHA512Hash("C12345") }
                );


            // Set composite keys1
            modelBuilder.Entity<CommissionTracking>()
                .HasKey(ct => new { ct.UserId, ct.CommissionId });

            modelBuilder.Entity<Favorite>()
                .HasKey(ct => new { ct.UserId, ct.ModId });

            modelBuilder.Entity<Log>()
                .HasKey(ct => new { ct.UserId, ct.ModId });

            modelBuilder.Entity<ModLike>()
                .HasKey(ct => new { ct.UserId, ct.ModId });

            modelBuilder.Entity<ModTag>()
                .HasKey(ct => new { ct.TagId, ct.ModId });

            modelBuilder.Entity<PurchasedMod>()
                .HasKey(ct => new { ct.UserId, ct.ModId });

            modelBuilder.Entity<PrivateMessage>()
                .HasOne(pm => pm.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(pm => pm.SenderId);

            modelBuilder.Entity<PrivateMessage>()
                .HasOne(pm => pm.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(pm => pm.ReceiverId);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Notifier)
                .WithMany(u => u.SentNotifications)
                .HasForeignKey(n => n.NotifierId);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Recipient)
                .WithMany(u => u.ReceivedNotifications)
                .HasForeignKey(n => n.RecipientId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Payer)
                .WithMany(u => u.Payments)
                .HasForeignKey(t => t.PayerId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Payee)
                .WithMany(u => u.Incomes)
                .HasForeignKey(t => t.PayeeId);

            modelBuilder.Entity<Commission>()
                .Property(c => c.Budget)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Mod>()
                .Property(m => m.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Counter>()
                .HasData(
                    new Counter { CounterId = 1, Value = 0 , CounterName = "Mod"},
                    new Counter { CounterId = 2, Value = 0, CounterName = "Commission" }
                );

            modelBuilder.Entity<CommissionStatus>()
               .HasData(new CommissionStatus { CommissionStatusId=  "s01", Status= "待接受"},
                        new CommissionStatus { CommissionStatusId = "s02", Status = "進行中" },
                        new CommissionStatus { CommissionStatusId = "s03", Status = "已完成" },
                        new CommissionStatus { CommissionStatusId = "s04", Status = "已取消" }
               );

            modelBuilder.Entity<Game>()
                .HasData(
                    new Game
                    {
                        GameId = "g001",
                        GameName = "Minecraft",
                        Description = "mcTest",
                        Thumbnail = "/GameImages/mcImg.jpg",
                        CreateTime = new DateTime(2023, 5, 27, 17, 21, 0)
                    },
                    new Game
                    {
                        GameId = "g002",
                        GameName = "Fortnite",
                        Description = "fnTest",
                        Thumbnail = "/GameImages/fnImg.jpg",
                        CreateTime = new DateTime(2023, 5, 28, 10, 30, 0)
                    },
                    new Game
                    {
                        GameId = "g003",
                        GameName = "Overwatch",
                        Description = "owTest",
                        Thumbnail = "/GameImages/owImg.jpg",
                        CreateTime = new DateTime(2023, 5, 29, 14, 15, 0)
                    }
                );

            modelBuilder.Entity<Tag>()
                .HasData(
                    new Tag { TagId = "t001", TagName = "劇情" },
                    new Tag { TagId = "t002", TagName = "數值" },
                    new Tag { TagId = "t003", TagName = "武器" },
                    new Tag { TagId = "t004", TagName = "道具" },
                    new Tag { TagId = "t005", TagName = "地圖" },
                    new Tag { TagId = "t006", TagName = "音樂" },
                    new Tag { TagId = "t007", TagName = "美術" },
                    new Tag { TagId = "t008", TagName = "程式" },
                    new Tag { TagId = "t009", TagName = "其他" }
                );

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var foreignKeys = entityType.GetForeignKeys();

                foreach (var foreignKey in foreignKeys)
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
                }
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
