using Microsoft.AspNetCore.Mvc;
using MyAuthDemo.Models;
using MyAuthDemo.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using MyAuthDemo.Data;

namespace MyAuthDemo.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _auth;
        private readonly AppDbContext _db;

        public AuthController(AuthService auth, AppDbContext db)
        {
            _auth = auth;
            _db = db;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Roles = _db.Roles.Select(r => new { r.Id, r.Name }).ToList();
            ViewBag.Positions = _db.Positions.Select(p => new { p.Id, p.Name }).ToList();
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            ViewBag.Roles = _db.Roles.Select(r => new { r.Id, r.Name }).ToList();
            ViewBag.Positions = _db.Positions.Select(p => new { p.Id, p.Name }).ToList();

            if (!ModelState.IsValid) return View(vm);

            string? avatarUrl = null;
            if (vm.Avatar != null && vm.Avatar.Length > 0)
            {
                var uploads = Path.Combine("wwwroot", "avatars");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.Avatar.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await vm.Avatar.CopyToAsync(stream);
                }
                avatarUrl = $"/avatars/{fileName}";
            }

            var (success, error) = await _auth.RegisterAsync(
                vm.Email, vm.Password, vm.Name, vm.RoleId, vm.PositionId, avatarUrl);

            if (!success)
            {
                ModelState.AddModelError("", error ?? "Gagal registrasi");
                return View(vm);
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _auth.ValidateUserAsync(vm.Email, vm.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Email/password salah");
                return View(vm);
            }

            // Ambil role & permissions
            var role = user.Role?.Name ?? "";

            var permissionNames = user.Role?.RolePermissions?
                .Select(rp => rp.Permission?.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))  // hindari null & string kosong
                .Select(name => name!)                            // karena sudah difilter, kita safe pakai '!'
                .ToList() ?? new List<string>();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role)
            };

            foreach (var permission in permissionNames)
            {
                claims.Add(new Claim("permission", permission));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Dashboard");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
