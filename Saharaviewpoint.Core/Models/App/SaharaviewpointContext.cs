using Microsoft.EntityFrameworkCore;

namespace Saharaviewpoint.Core.Models.App;

public class SaharaviewpointContext : DbContext
{
    public SaharaviewpointContext()
    { }

    public SaharaviewpointContext(DbContextOptions<SaharaviewpointContext> options) : base(options)
    {
    }

    public required DbSet<Role> Roles { get; set; }
    public required DbSet<User> Users { get; set; }
    public required DbSet<UserRole> UserRoles { get; set; }
    public required DbSet<RefreshToken> RefreshTokens { get; set; }
    public required DbSet<Project> Projects { get; set; }
    public required DbSet<ProjectType> ProjectTypes { get; set; }
    public required DbSet<Document> Documents { get; set; }
    public required DbSet<PMInvitation> PMInvitations { get; set; }
    public required DbSet<Code> Codes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("dbo");

        builder
            .Entity<User>()
            .ToTable(p => p.HasCheckConstraint("CK_User_Type", "[Type] IN ('Business', 'Client', 'Manager')"))
            .HasMany(u => u.Projects)
            .WithOne(u => u.Assignee);

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