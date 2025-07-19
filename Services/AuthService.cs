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

        public async Task<(bool Success, string? Error)> RegisterAsync(
            string email, string password, string? name,
            int? roleId, int? positionId, string? avatarUrl)
        {
            if (await _repo.EmailExistsAsync(email))
                return (false, "Email sudah terdaftar.");
            var hash = _hasher.HashPassword(email, password);
            var user = new User
            {
                Email = email,
                PasswordHash = hash,
                Name = name,
                RoleId = roleId,
                PositionId = positionId,
                AvatarUrl = avatarUrl
            };
            await _repo.AddAsync(user);
            return (true, null);
        }

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null)
                return null;
            var res = _hasher.VerifyHashedPassword(email, user.PasswordHash, password);
            if (res == PasswordVerificationResult.Success)
            {
                // Include role/position info (opsional, bisa eager loading)
                user.Role = _db.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                user.Position = _db.Positions.FirstOrDefault(p => p.Id == user.PositionId);
                return user;
            }
            return null;
        }
    }
}
