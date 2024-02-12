using Microsoft.EntityFrameworkCore;

namespace Saharaviewpoint.Core.Models.App;

public class SaharaviewpointContext : DbContext
{
    public SaharaviewpointContext()
    { }

    public SaharaviewpointContext(DbContextOptions<SaharaviewpointContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectType> ProjectTypes { get; set; }
    public DbSet<Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("dbo");

        builder
            .Entity<User>()
            .ToTable(p => p.HasCheckConstraint("CK_User_Type", "[Type] IN ('Business', 'Client', 'Manager')"));

        builder.Entity<UserRole>(entity =>
        {
            entity.HasKey(t => new { t.RoleId, t.UserId });
        });

        builder.Entity<Project>()
            .ToTable(p => p.HasCheckConstraint("CK_Project_Status", "[Status] IN ('Requested', 'In Progress', 'Completed')"));

        builder.Entity<Document>()
            .ToTable(p => p.HasCheckConstraint("CK_Document_Type", "[Type] IN ('Image', 'PDF', 'Word Document', 'Unknown')"));
    }
}