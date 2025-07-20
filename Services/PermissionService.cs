using System.Collections.Generic;
using System.Linq;
using MyAuthDemo.Data;
using Microsoft.EntityFrameworkCore;

namespace MyAuthDemo.Services
{
    public class PermissionService
    {
        private readonly AppDbContext _db;
        public PermissionService(AppDbContext db) { _db = db; }

        public bool UserHasPermission(int userId, string permissionName)
{
    var user = _db.Users
        .Include(u => u.Role)
            .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
        .FirstOrDefault(u => u.Id == userId);

    if (user == null || user.Role == null || user.Role.RolePermissions == null)
        return false;

    return user?.Role?.RolePermissions != null &&
       user.Role.RolePermissions.Any(rp => rp.Permission?.Name == permissionName);

}

public List<string> GetUserPermissions(int userId)
{
    var user = _db.Users
        .Include(u => u.Role)
            .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
        .FirstOrDefault(u => u.Id == userId);

    if (user == null || user.Role == null || user.Role.RolePermissions == null)
        return new List<string>();

    return user?.Role?.RolePermissions?
        .Where(rp => rp.Permission != null)
        .Select(rp => rp.Permission!.Name)
        .ToList() ?? new List<string>();

}



    }
}
