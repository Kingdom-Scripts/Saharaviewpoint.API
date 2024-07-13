using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Saharaviewpoint.Models.App.Constants;
using Saharaviewpoint.Models.Constants;

namespace Saharaviewpoint.Models.App;

public class SaharaviewpointContext : DbContext
{
    public SaharaviewpointContext()
    {
    }

    public SaharaviewpointContext(DbContextOptions<SaharaviewpointContext> options) : base(options)
    {
    }

    public required DbSet<Role> Roles { get; set; }
    public required DbSet<User> Users { get; set; }
    public required DbSet<UserRole> UserRoles { get; set; }
    public required DbSet<RefreshToken> RefreshTokens { get; set; }
    public required DbSet<Project> Projects { get; set; }
    public required DbSet<ProjectLog> ProjectLogs { get; set; }
    public required DbSet<ProjectType> ProjectTypes { get; set; }
    public required DbSet<ProjectTaskApproval> ProjectTaskApprovals { get; set; }
    public required DbSet<Document> Documents { get; set; }
    public required DbSet<PMInvitation> PMInvitations { get; set; }
    public required DbSet<Code> Codes { get; set; }
    public required DbSet<SvpTask> Tasks { get; set; }
    public required DbSet<TaskAttachment> TaskAttachments { get; set; }
    public required DbSet<EpicTask> EpicTasks { get; set; }
    public required DbSet<TaskLog> TaskLogs { get; set; }
    public required DbSet<TaskComment> TaskComments { get; set; }
    public required DbSet<Login> Logins { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("dbo");

        builder
            .Entity<User>()
            .ToTable(p => p.HasCheckConstraint("CK_User_Type", $"[Type] IN ({UserTypes.DB_CONSTRAINT})"))
            .HasMany(u => u.Projects)
            .WithOne(u => u.Assignee);

        builder.Entity<UserRole>(entity => { entity.HasKey(t => new { t.RoleId, t.UserId }); });

        builder.Entity<Project>()
            .ToTable(p =>
                p.HasCheckConstraint("CK_Project_Status", $"[Status] IN ('{ProjectStatuses.Requested}', '{ProjectStatuses.InProgress}', '{ProjectStatuses.Completed}')"))
            .Property(p => p.FolderNames)
            .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<string>>(v)!);

        var valueComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        // Configure the FolderNames property to use the value comparer
        builder
            .Entity<Project>()
            .Property(e => e.FolderNames)
            .Metadata
            .SetValueComparer(valueComparer);

        builder.Entity<Project>()
            .HasOne(p => p.CreatedBy)
            .WithMany()
            .HasForeignKey(p => p.CreatedById);

        builder.Entity<Project>()
            .HasOne(p => p.UpdatedBy)
            .WithMany()
            .HasForeignKey(p => p.UpdatedById);

        builder.Entity<Project>()
            .HasOne(p => p.DeletedBy)
            .WithMany()
            .HasForeignKey(p => p.DeletedById);

        builder.Entity<ProjectLog>()
            .ToTable(pl => pl.HasCheckConstraint("CK_Project_Log", $"[Type] IN ('{ProjectLogTypes.Create}', '{ProjectLogTypes.Update}', '{ProjectLogTypes.Delete}', '{ProjectLogTypes.Assignment}', '{ProjectLogTypes.StatusChange}', '{ProjectLogTypes.Approvals}')"))
             .HasOne(t => t.CreatedBy)
            .WithMany()
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure the foreign key for Requester
        builder.Entity<ProjectTaskApproval>()
            .HasOne(pta => pta.Requester)
            .WithMany()
            .HasForeignKey(pta => pta.RequesterId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

        // Configure the foreign key for FulfilledBy
        builder.Entity<ProjectTaskApproval>()
            .HasOne(pta => pta.FulfilledBy)
            .WithMany()
            .HasForeignKey(pta => pta.FulfilledById)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading

        builder.Entity<Document>()
            .ToTable(p =>
                p.HasCheckConstraint("CK_Document_Type", "[Type] IN ('Image', 'PDF', 'Word Document', 'Unknown')"));

        builder.Entity<SvpTask>()
            .ToTable(p => p.HasCheckConstraint("CK_Task_Type",
                $"[Type] IN ('{TaskTypeEnum.EPIC}', '{TaskTypeEnum.TASK}', '{TaskTypeEnum.SUBTASK}')"))
            .ToTable(p => p.HasCheckConstraint("CK_Task_Status",
                $"[Status] IN ('{TaskStatusEnum.TODO}', '{TaskStatusEnum.IN_PROGRESS}', '{TaskStatusEnum.COMPLETED}')"))
            .HasOne(t => t.CreatedBy)
            .WithMany()
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<TaskAttachment>()
            .HasOne(ta => ta.Task)
            .WithMany(t => t.TaskAttachments)
            .HasForeignKey(ta => ta.TaskId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<EpicTask>()
            .HasIndex(et => et.TaskId);

        builder.Entity<EpicTask>()
            .HasOne(ep => ep.CreatedBy)
            .WithMany()
            .HasForeignKey(ep => ep.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<TaskLog>()
            .HasOne(tl => tl.CreatedBy)
            .WithMany()
            .HasForeignKey(tl => tl.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<TaskComment>()
            .HasOne(tl => tl.CreatedBy)
            .WithMany()
            .HasForeignKey(tl => tl.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);
    }
}