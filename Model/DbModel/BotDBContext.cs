using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BotConsole.Model
{
    public partial class BotDBContext : DbContext
    {
        public virtual DbSet<Action> Action { get; set; }
        public virtual DbSet<ActionUserSend> ActionUserSend { get; set; }
        public virtual DbSet<HomeWork> HomeWork { get; set; }
        public virtual DbSet<HomeWorkUserSend> HomeWorkUserSend { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserHomeWork> UserHomeWork { get; set; }
        public virtual DbSet<UserHomeWorkReminder> UserHomeWorkReminder { get; set; }

        public BotDBContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Action>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<ActionUserSend>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.ActionUserSend)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ActionUserSend)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<HomeWork>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
        }
    }
}
