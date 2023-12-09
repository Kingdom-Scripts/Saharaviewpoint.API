using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Saharaviewpoint.Core.Models.App
{
    public partial class ShareviewpointContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public ShareviewpointContext() { }

        public ShareviewpointContext(DbContextOptions<ShareviewpointContext> options) : base(options) { }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(t => new { t.RoleId, t.UserId });
                //entity.HasOne(ur => ur.Role)
                //  .WithMany(r => r.UserRoles)
                //  .HasForeignKey(ur => ur.RoleId);
            });

            //modelBuilder.Entity<Role>(entity =>
            //{
            //    // Manually configure the inverse relationship
            //    entity.HasMany(r => r.UserRoles)
            //          .WithOne(ur => ur.Role)
            //          .HasForeignKey(ur => ur.RoleId);
            //});
        }
    }
}