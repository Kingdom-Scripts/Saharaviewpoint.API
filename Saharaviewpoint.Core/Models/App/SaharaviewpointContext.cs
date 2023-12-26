using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Infrastructure.DependencyResolution;

namespace Saharaviewpoint.Core.Models.App;

public partial class SaharaviewpointContext : DbContext
{
    public SaharaviewpointContext() { }

    public SaharaviewpointContext(DbContextOptions<SaharaviewpointContext> options) : base(options) { }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("dbo");

        modelBuilder
            .Entity<User>()
            .Property(u => u.Type)
            .HasConversion<int>();

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(t => new { t.RoleId, t.UserId });
        });
    }
}