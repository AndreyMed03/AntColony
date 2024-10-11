using API_Server.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace API_Server.Contexts
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_User");

                entity.ToTable("Users");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.ColonyId)
                    .HasColumnName("colony_id");

                entity.Property(e => e.Username)
                    .HasMaxLength(20)
                    .HasColumnName("username");

                entity.Property(e => e.HashedPassword)
                    .HasMaxLength(64)
                    .HasColumnName("hashed_password");

                entity.Property(e => e.Email)
                    .HasMaxLength(40)
                    .HasColumnName("email");

                entity.Property(e => e.PositionInTheLeaderboard)
                    .HasColumnName("position_in_the_leaderboard");
            });
        }
    }
}
