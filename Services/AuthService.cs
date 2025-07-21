using MyAuthDemo.Models;
using MyAuthDemo.Repositories;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using MyAuthDemo.Data;

namespace MyAuthDemo.Services
{
    public class AuthService
    {
        private readonly IUserRepository _repo;
        private readonly AppDbContext _db;
        private readonly PasswordHasher<string> _hasher = new();

        public AuthService(IUserRepository repo, AppDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        public async Task<(bool success, string? error)> RegisterAsync(
            string email, string password, string name, int? roleId, int? positionId, string? avatarUrl)
        {
            if (_db.Users.Any(u => u.Email == email))
                return (false, "Email sudah digunakan");

            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Name = name,
                RoleId = roleId, // bisa null
                PositionId = positionId, // bisa null
                AvatarUrl = avatarUrl
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return (true, null);
        }


        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null)
                return null;

            // Gunakan BCrypt untuk verifikasi
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (isValid)
            {
                user.Role = _db.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                user.Position = _db.Positions.FirstOrDefault(p => p.Id == user.PositionId);
                return user;
            }

            return null;
        }

    }
}
