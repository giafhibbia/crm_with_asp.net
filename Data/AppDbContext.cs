using Microsoft.EntityFrameworkCore;
using MyAuthDemo.Models;
using MyAuthDemo.Models.Leads;
using MyAuthDemo.Models.Region;

namespace MyAuthDemo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<FcmToken> FcmTokens { get; set; }
          public DbSet<Province> Provinces { get; set; }
        public DbSet<Regency> Regencies { get; set; }
        public DbSet<Lead> Leads { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationships for Groups
            // Group → User
            modelBuilder.Entity<Group>()
                .HasOne(g => g.UserCreate)
                .WithMany()
                .HasForeignKey(g => g.UserIdCreate)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Groups_UserIdCreate");

            modelBuilder.Entity<Group>()
                .HasOne(g => g.UserUpdate)
                .WithMany()
                .HasForeignKey(g => g.UserIdUpdate)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Groups_UserIdUpdate");


            // User → Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Users_RoleId");

            // User → Position
            modelBuilder.Entity<User>()
                .HasOne(u => u.Position)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.PositionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Users_PositionId");

            // RolePermission → Role & Permission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .HasConstraintName("FK_RolePermissions_RoleId");

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .HasConstraintName("FK_RolePermissions_PermissionId");
                
            modelBuilder.Entity<Province>()
                .ToTable("reg_provinces")
                .HasKey(p => p.Id);

            modelBuilder.Entity<Province>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(255);

            // Regency
            modelBuilder.Entity<Regency>()
                .ToTable("reg_regencies")
                .HasKey(r => r.Id);

            modelBuilder.Entity<Regency>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Regency>()
                .HasOne(r => r.Province)
                .WithMany()
                .HasForeignKey(r => r.ProvinceId)
                .OnDelete(DeleteBehavior.Cascade);


                

            // Lead → Group
            modelBuilder.Entity<Lead>()
                .HasOne(z => z.Group)
                .WithMany(g => g.Leads)
                .HasForeignKey(z => z.GroupId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Leads_Groups_GroupId");


        }

    }
}
