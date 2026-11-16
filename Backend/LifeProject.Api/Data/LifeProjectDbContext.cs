
using LifeProject.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LifeProject.Api.Data;

public class LifeProjectDbContext : DbContext
{
    public LifeProjectDbContext(
        DbContextOptions<LifeProjectDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<House> Houses => Set<House>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.DisplayName)
                .HasMaxLength(40)
                .IsRequired();

            entity.HasIndex(p => p.DisplayName)
                .IsUnique();

            entity.HasOne(p => p.House)
                .WithOne(h => h.Player)
                .HasForeignKey<House>(h => h.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<House>(entity =>
        {
            entity.HasKey(h => h.Id);
        });
    }
}
