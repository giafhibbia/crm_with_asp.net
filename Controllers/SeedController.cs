using Microsoft.AspNetCore.Mvc;
using MyAuthDemo.Data;
using MyAuthDemo.Models;
using System.Linq;

namespace MyAuthDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SeedController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("seed")]
        public IActionResult SeedData()
        {
            if (!_context.Roles.Any())
            {
                var roles = new[]
                {
                    new Role { Name = "Admin", Level = "C-Level" },
                    new Role { Name = "Marketing", Level = "Staff" },
                    new Role { Name = "Teknisi", Level = "Staff" },
                    new Role { Name = "Approval", Level = "Manager" },
                    new Role { Name = "CFO", Level = "C-Level" },
                    new Role { Name = "CMO", Level = "C-Level" }
                };
                _context.Roles.AddRange(roles);
                _context.SaveChanges();
            }

            if (!_context.Positions.Any())
            {
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
                _context.Positions.AddRange(positions);
                _context.SaveChanges();
            }

            if (!_context.Permissions.Any())
            {
                var permissions = new[]
                {
                    new Permission { Name = "user.create" },
                    new Permission { Name = "user.edit" },
                    new Permission { Name = "user.delete" },
                    new Permission { Name = "user.approve" },
                    new Permission { Name = "role.manage" },
                    new Permission { Name = "position.manage" }
                };
                _context.Permissions.AddRange(permissions);
                _context.SaveChanges();
            }

            if (!_context.RolePermissions.Any())
            {
                var adminRole = _context.Roles.First(r => r.Name == "Admin");
                var marketingRole = _context.Roles.First(r => r.Name == "Marketing");

                var userCreate = _context.Permissions.First(p => p.Name == "user.create");
                var userEdit = _context.Permissions.First(p => p.Name == "user.edit");
                var userDelete = _context.Permissions.First(p => p.Name == "user.delete");
                var userApprove = _context.Permissions.First(p => p.Name == "user.approve");
                var roleManage = _context.Permissions.First(p => p.Name == "role.manage");
                var positionManage = _context.Permissions.First(p => p.Name == "position.manage");

                var rolePermissions = new[]
                {
                    new RolePermission { RoleId = adminRole.Id, PermissionId = userCreate.Id },
                    new RolePermission { RoleId = adminRole.Id, PermissionId = userEdit.Id },
                    new RolePermission { RoleId = adminRole.Id, PermissionId = userDelete.Id },
                    new RolePermission { RoleId = adminRole.Id, PermissionId = userApprove.Id },
                    new RolePermission { RoleId = adminRole.Id, PermissionId = roleManage.Id },
                    new RolePermission { RoleId = adminRole.Id, PermissionId = positionManage.Id },

                    new RolePermission { RoleId = marketingRole.Id, PermissionId = userEdit.Id },
                    new RolePermission { RoleId = marketingRole.Id, PermissionId = userApprove.Id }
                };

                _context.RolePermissions.AddRange(rolePermissions);
                _context.SaveChanges();
            }

            return Ok("Seed data inserted successfully!");
        }
    }
}
