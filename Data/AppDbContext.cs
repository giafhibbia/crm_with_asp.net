using Microsoft.EntityFrameworkCore;
using MyAuthDemo.Models;

namespace MyAuthDemo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Tabel Users, Roles, Positions, Permissions, RolePermissions
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        // Tabel tambahan untuk Group dan Lead
        public DbSet<Group> Groups { get; set; }
        public DbSet<Lead> Leads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- User-Role (Many Users to One Role) ---
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // Jangan hapus Role kalau ada User

            // --- User-Position (Many Users to One Position) ---
            modelBuilder.Entity<User>()
                .HasOne(u => u.Position)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Role - RolePermission (One Role to Many RolePermissions) ---
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade); // Hapus semua RolePermissions jika Role dihapus

            // --- Permission - RolePermission (One Permission to Many RolePermissions) ---
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Group - Lead (One Group to Many Leads) ---
            modelBuilder.Entity<Group>()
                .HasMany(g => g.Leads)
                .WithOne(l => l.Group)
                .HasForeignKey(l => l.GroupId)
                .OnDelete(DeleteBehavior.SetNull); // Jika Group dihapus, set GroupId Lead jadi null

            // --- Group - User (UserCreate) ---
            modelBuilder.Entity<Group>()
                .HasOne(g => g.UserCreate)
                .WithMany()
                .HasForeignKey(g => g.UserIdCreate)
                .OnDelete(DeleteBehavior.Restrict); // User yang buat Group tidak bisa dihapus tanpa reset fk

            // --- Group - User (UserUpdate) ---
            modelBuilder.Entity<Group>()
                .HasOne(g => g.UserUpdate)
                .WithMany()
                .HasForeignKey(g => g.UserIdUpdate)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Lead - User (Creator) ---
            modelBuilder.Entity<Lead>()
                .HasOne(l => l.User)
                .WithMany(u => u.Leads)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict); // User tidak bisa dihapus jika masih punya Lead

            // Optional: Jika Anda ingin memperjelas bahwa property navigasi harus virtual supaya lazy loading bisa berjalan
            // Bisa ditambahkan secara eksplisit pada model, tapi EF Core tidak mewajibkan
        }
    }
}
