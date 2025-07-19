using Microsoft.EntityFrameworkCore; // <-- ini wajib ada
using MyAuthDemo.Data;
using MyAuthDemo.Models;
using System;
using System.Linq;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;


namespace MyAuthDemo.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            try
            {
                Console.WriteLine("SeedData.Initialize started");

                // Pastikan database sudah ada dan migrasi sudah dijalankan
                context.Database.Migrate();

                // Seed Roles
                if (!context.Roles.Any())
                {
                    Console.WriteLine("Seeding Roles...");
                    var roles = new[]
                    {
                        new Role { Name = "Admin", Level = "C-Level" },
                        new Role { Name = "Marketing", Level = "Staff" },
                        new Role { Name = "Teknisi", Level = "Staff" },
                        new Role { Name = "Approval", Level = "Manager" },
                        new Role { Name = "CFO", Level = "C-Level" },
                        new Role { Name = "CMO", Level = "C-Level" }
                    };
                    context.Roles.AddRange(roles);
                    context.SaveChanges();
                    Console.WriteLine("Roles seeded");
                }
                else
                {
                    Console.WriteLine("Roles already exist, skipping seeding");
                }

                // Seed Positions
                if (!context.Positions.Any())
                {
                    Console.WriteLine("Seeding Positions...");
                    var positions = new[]
                    {
                        new Position { Name = "Staff" },
                        new Position { Name = "Supervisor" },
                        new Position { Name = "Manager" },
                        new Position { Name = "General Manager" },
                        new Position { Name = "Head of Finance & Administration" },
                        new Position { Name = "Finance & Administration Manager" },
                        new Position { Name = "Finance & Administration Supervisor" },
                        new Position { Name = "CFO" },
                        new Position { Name = "COO" },
                        new Position { Name = "CEO" },
                        new Position { Name = "CMO" }
                    };
                    context.Positions.AddRange(positions);
                    context.SaveChanges();
                    Console.WriteLine("Positions seeded");
                }
                else
                {
                    Console.WriteLine("Positions already exist, skipping seeding");
                }

                // Seed Permissions
                if (!context.Permissions.Any())
                {
                    Console.WriteLine("Seeding Permissions...");
                    var permissions = new[]
                    {
                        new Permission { Name = "user.create" },
                        new Permission { Name = "user.edit" },
                        new Permission { Name = "user.delete" },
                        new Permission { Name = "user.approve" },
                        new Permission { Name = "role.manage" },
                        new Permission { Name = "position.manage" }
                    };
                    context.Permissions.AddRange(permissions);
                    context.SaveChanges();
                    Console.WriteLine("Permissions seeded");
                }
                else
                {
                    Console.WriteLine("Permissions already exist, skipping seeding");
                }

                // Seed RolePermissions
                if (!context.RolePermissions.Any())
                {
                    Console.WriteLine("Seeding RolePermissions...");
                    var adminRole = context.Roles.First(r => r.Name == "Admin");
                    var marketingRole = context.Roles.First(r => r.Name == "Marketing");

                    var userCreate = context.Permissions.First(p => p.Name == "user.create");
                    var userEdit = context.Permissions.First(p => p.Name == "user.edit");
                    var userDelete = context.Permissions.First(p => p.Name == "user.delete");
                    var userApprove = context.Permissions.First(p => p.Name == "user.approve");
                    var roleManage = context.Permissions.First(p => p.Name == "role.manage");
                    var positionManage = context.Permissions.First(p => p.Name == "position.manage");

                    var rolePermissions = new[]
                    {
                        new RolePermission { RoleId = adminRole.Id, PermissionId = userCreate.Id },
                        new RolePermission { RoleId = adminRole.Id, PermissionId = userEdit.Id },
                        new RolePermission { RoleId = adminRole.Id, PermissionId = userDelete.Id },
                        new RolePermission { RoleId = adminRole.Id, PermissionId = userApprove.Id },
                        new RolePermission { RoleId = adminRole.Id, PermissionId = roleManage.Id },
                        new RolePermission { RoleId = adminRole.Id, PermissionId = positionManage.Id },

                        // Marketing hanya bisa edit dan approve user
                        new RolePermission { RoleId = marketingRole.Id, PermissionId = userEdit.Id },
                        new RolePermission { RoleId = marketingRole.Id, PermissionId = userApprove.Id },
                    };

                    context.RolePermissions.AddRange(rolePermissions);
                    context.SaveChanges();
                    Console.WriteLine("RolePermissions seeded");
                }
                else
                {
                    Console.WriteLine("RolePermissions already exist, skipping seeding");
                }

                Console.WriteLine("SeedData.Initialize finished");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during SeedData.Initialize: " + ex.Message);
                throw;
            }
        }
    }
}
