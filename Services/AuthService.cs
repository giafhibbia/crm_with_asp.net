// Services/AuthService.cs
using MyAuthDemo.Models;
// using MyAuthDemo.Repositories; // <<< Hapus baris ini
// using Microsoft.AspNetCore.Identity; // <<< Hapus baris ini (jika tidak ada lagi penggunaan PasswordHasher<string>)
using System.ComponentModel.DataAnnotations;
using MyAuthDemo.Data;
using Microsoft.EntityFrameworkCore; // <<< Pastikan ini ada
using BCrypt.Net; // <<< Pastikan ini ada

namespace MyAuthDemo.Services
{
    public class AuthService
    {
        // private readonly IUserRepository _repo; // <<< Hapus baris ini
        private readonly AppDbContext _db;
        // private readonly PasswordHasher<string> _hasher = new(); // <<< Hapus baris ini jika tidak terpakai sama sekali

        // Sesuaikan konstruktor: hanya menerima AppDbContext
        public AuthService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(bool success, string? error)> RegisterAsync(
            string email, string password, string name, int? roleId, int? positionId, string? avatarUrl)
        {
            // Gunakan _db langsung untuk cek email
            if (await _db.Users.AnyAsync(u => u.Email == email))
                return (false, "Email sudah digunakan");

            var user = new User
            {
                Email = email,
                // Menggunakan PasswordHash untuk menyimpan hash.
                // Pastikan kolom di database Anda juga bernama PasswordHash.
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Name = name,
                RoleId = roleId,
                PositionId = positionId,
                AvatarUrl = avatarUrl
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            // PENTING: Lakukan eager loading relasi yang dibutuhkan
            // Ini akan memuat Role, RolePermissions, dan Permission mereka dalam satu query
            var user = await _db.Users
                                .Include(u => u.Role) // Muat data Role
                                    .ThenInclude(r => r!.RolePermissions) // Lalu muat RolePermissions dari Role
                                        .ThenInclude(rp => rp.Permission) // Lalu muat Permission dari RolePermission
                                .Include(u => u.Position) // Muat data Position
                                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null; // Pengguna tidak ditemukan

            // Pastikan properti yang menyimpan hash (user.PasswordHash) tidak null atau kosong
            // Ini crucial agar BCrypt.Net.BCrypt.Verify tidak error "Invalid salt"
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                Console.WriteLine($"WARNING: User {user.Email} has empty/null PasswordHash. Cannot verify password.");
                return null; // Atau throw exception jika ini kondisi error yang harus ditangani
            }

            // Gunakan BCrypt untuk verifikasi password.
            // Pastikan user.PasswordHash adalah properti yang benar yang menyimpan hash password.
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            
            if (isValid)
            {
                // user objek sudah terisi dengan Role dan Position karena eager loading
                return user;
            }

            return null; // Password tidak cocok
        }
    }
}