using MyAuthDemo.Data;
using MyAuthDemo.Models;

namespace MyAuthDemo.Data.Seeders
{
    public static class SeedRolePermissionData
    {
        public static void Initialize(AppDbContext db)
        {
            Console.WriteLine("🚀 Seeding roles, permissions, and role_permissions...");

            // Lewati jika sudah ada data
            if (db.Roles.Any() || db.Permissions.Any())
            {
                Console.WriteLine("✅ Roles or permissions already seeded.");
                return;
            }

            // 1. Roles
            var roles = new List<Role>
            {
                new Role { Name = "Super Admin" },
                new Role { Name = "Admin" },
                new Role { Name = "User" },
            };
            db.Roles.AddRange(roles);
            db.SaveChanges();

            // 2. Permissions (menu.action format)
            var menus = new[] { "dashboard", "user", "lead", "customer", "group" };
            var actions = new[] { "view", "read", "create", "update", "delete" };

            var permissions = new List<Permission>();
            foreach (var menu in menus)
            {
                foreach (var action in actions)
                {
                    permissions.Add(new Permission
                    {
                        Name = $"{menu}.{action}"
                    });
                }
            }
            db.Permissions.AddRange(permissions);
            db.SaveChanges();

            // 3. Assign all permissions to "Super Admin"
            var superAdminRoleId = db.Roles.FirstOrDefault(r => r.Name == "Super Admin")?.Id;
            if (superAdminRoleId == null)
            {
                Console.WriteLine("❌ Failed to find Super Admin role.");
                return;
            }

            var allPermissionIds = db.Permissions.Select(p => p.Id).ToList();
            var rolePermissions = allPermissionIds.Select(pid => new RolePermission
            {
                RoleId = superAdminRoleId.Value,
                PermissionId = pid
            }).ToList();

            db.RolePermissions.AddRange(rolePermissions);
            db.SaveChanges();

            Console.WriteLine("✅ Seeding completed with full permissions.");
        }
    }
}
