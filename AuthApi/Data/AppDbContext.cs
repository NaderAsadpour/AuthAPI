using AuthAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasOne(u => u.RefreshToken)
            .WithOne(r => r.User)
            .HasForeignKey<RefreshToken>(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
