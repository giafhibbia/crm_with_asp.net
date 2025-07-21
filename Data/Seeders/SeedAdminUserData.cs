using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using MyAuthDemo.Data;
using MyAuthDemo.Models;

namespace MyAuthDemo.Data.Seeders
{
    public static class SeedAdminUserData
    {
        public static void Initialize(AppDbContext context)
        {
            Console.WriteLine("🟡 Memeriksa apakah admin sudah ada...");

            // Cek apakah sudah ada user admin
            if (context.Users.Any(u => u.Email == "admin@example.com"))
            {
                Console.WriteLine("✅ Admin sudah ada, skip seeding.");
                return;
            }

            Console.WriteLine("➕ Membuat user admin default...");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Admin123!");

            var adminUser = new User
            {
                Email = "admin@example.com",
                Password = hashedPassword,
                PasswordHash = hashedPassword,
                Name = "Super Admin",
                RoleId = null,         // pastikan role ID 1 adalah Superadmin
                PositionId = 1,     // sesuaikan jika perlu
                AvatarUrl = null
            };

            context.Users.Add(adminUser);
            context.SaveChanges();

            Console.WriteLine("✅ Admin user berhasil ditambahkan.");
        }
    }
}
