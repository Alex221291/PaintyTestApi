using Microsoft.EntityFrameworkCore;
using PaintyTestApi.Auth;
using PaintyTestApi.ConstantsData;
using PaintyTestApi.Models;

namespace PaintyTestApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Users)
            .WithMany(u => u.Friends);
    }
}