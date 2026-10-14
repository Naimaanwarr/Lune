using Lune.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Lune.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Resident> Residents => Set<Resident>();
    public DbSet<BehaviorType> BehaviorTypes => Set<BehaviorType>();
    public DbSet<Measure> Measures => Set<Measure>();
    public DbSet<Observation> Observations => Set<Observation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Resident>(e =>
        {
            e.Property(r => r.FirstName).HasMaxLength(50);
            e.Property(r => r.LastName).HasMaxLength(50);
            e.Property(r => r.Department).HasMaxLength(100);
            e.Property(r => r.Gender).HasConversion<string>().HasMaxLength(20);
            e.Property(r => r.DementiaType).HasConversion<string>().HasMaxLength(30);
        });

        modelBuilder.Entity<BehaviorType>(e =>
        {
            e.Property(b => b.Name).HasMaxLength(50);
            e.Property(b => b.ColorHex).HasMaxLength(7);
        });

        modelBuilder.Entity<Measure>(e =>
        {
            e.Property(m => m.Name).HasMaxLength(100);
            e.Property(m => m.Icon).HasMaxLength(30);
        });

        modelBuilder.Entity<Observation>(e =>
        {
            e.Property(o => o.Comment).HasMaxLength(500);
            e.HasIndex(o => new { o.ResidentId, o.ObservedAt });

            e.HasOne(o => o.Resident)
                .WithMany(r => r.Observations)
                .HasForeignKey(o => o.ResidentId);
        });
    }
}