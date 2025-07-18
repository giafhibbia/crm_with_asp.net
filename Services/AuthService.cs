using MyAuthDemo.Models;
using MyAuthDemo.Repositories;
using Microsoft.AspNetCore.Identity;

namespace MyAuthDemo.Services
{
    public class AuthService
    {
        private readonly IUserRepository _repo;
        private readonly PasswordHasher<string> _hasher = new();

        public AuthService(IUserRepository repo) { _repo = repo; }

        public async Task<(bool Success, string? Error)> RegisterAsync(string email, string password, string? name)
        {
            if (await _repo.EmailExistsAsync(email))
                return (false, "Email sudah terdaftar.");
            var hash = _hasher.HashPassword(email, password);
            var user = new User { Email = email, PasswordHash = hash, Name = name };
            await _repo.AddAsync(user);
            return (true, null);
        }

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null)
                return null;
            var res = _hasher.VerifyHashedPassword(email, user.PasswordHash, password);
            return res == PasswordVerificationResult.Success ? user : null;
        }
    }
}
