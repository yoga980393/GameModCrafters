using Microsoft.EntityFrameworkCore;
using GameModCrafters.Models;

namespace GameModCrafters.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            modelBuilder.Entity<PrivateMessage>()
                .HasOne(pm => pm.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(pm => pm.SenderId);

            modelBuilder.Entity<PrivateMessage>()
                .HasOne(pm => pm.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(pm => pm.ReceiverId);

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
                    new Counter { CounterId = 1, Value = 0 , Name = "Mod"},
                    new Counter { CounterId = 2, Value = 0, Name = "Commission" }
                );

            modelBuilder.Entity<CommissionStatus>()
               .HasData(new CommissionStatus { CommissionStatusId= "s01", Status= "待接受"},
                        new CommissionStatus { CommissionStatusId = "s02", Status = "進行中" },
                        new CommissionStatus { CommissionStatusId = "s03", Status = "已完成" },
                        new CommissionStatus { CommissionStatusId = "s04", Status = "已取消" }
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
