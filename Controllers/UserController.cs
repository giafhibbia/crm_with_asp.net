using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAuthDemo.Data;
using MyAuthDemo.Services;
using MyAuthDemo.Models;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyAuthDemo.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly PermissionService _permissionService;
        private readonly AppDbContext _db;

        public UserController(PermissionService permissionService, AppDbContext db)
        {
            _permissionService = permissionService;
            _db = db;
        }

        public IActionResult Index(int page = 1, int pageSize = 10, string search = "", string role = "")
        {
            var query = _db.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Name.Contains(search) || u.Email.Contains(search));
            }

            if (!string.IsNullOrEmpty(role))
            {
                int roleId;
                if (int.TryParse(role, out roleId))
                {
                    query = query.Where(u => u.RoleId == roleId);
                }
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var users = query
                .OrderBy(u => u.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResult<User>
            {
                Items = users,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Search = search,
                Status = role
            };

            // Buat dropdown role filter
            ViewBag.RoleOptions = _db.Roles
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
                .ToList();

            return View(result);
        }


        public IActionResult Create()
        {
            // Tidak perlu lagi mengisi ViewBag.Roles di sini, karena Select2 akan mengambilnya via AJAX
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Jika model tidak valid, kita tidak perlu memuat ulang data Roles
                // karena Select2 akan mengambilnya dari API.
                return View(model);
            }

            // Cek apakah email sudah ada
            if (await _db.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(model);
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                RoleId = model.RoleId,
                PositionId = model.PositionId // Tambahkan PositionId
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(); // Gunakan async SaveChanges
            TempData["SuccessMessage"] = "User created successfully!";
            return RedirectToAction("Index");
        }

        // ... (Aksi Edit, Delete, Profile, GetUsers, EditProfile yang sudah ada) ...


        // --- New API Endpoints for Select2 (Server-side) ---

        [HttpGet]
        public async Task<IActionResult> GetRolesForSelect2(string search, int page)
        {
            int pageSize = 10; // Jumlah item per halaman
            var query = _db.Roles.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Name != null && r.Name.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var results = await query
                .OrderBy(r => r.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new { id = r.Id, text = r.Name })
                .ToListAsync();

            return Json(new { results = results, pagination = new { more = (page * pageSize) < totalCount } });
        }

        [HttpGet]
        public async Task<IActionResult> GetPositionsForSelect2(string search, int page)
        {
            int pageSize = 10; // Jumlah item per halaman
            var query = _db.Positions.AsQueryable(); // Pastikan ada DbSet<Position> di AppDbContext

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name != null && p.Name.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var results = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new { id = p.Id, text = p.Name })
                .ToListAsync();

            return Json(new { results = results, pagination = new { more = (page * pageSize) < totalCount } });
        }

        public IActionResult Edit(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null) return NotFound();

            var vm = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name ?? "",
                Email = user.Email,
                Password = "", // Kosongkan agar tidak tampil password hash
                RoleId = user.RoleId ?? 0
            };

            ViewBag.Roles = _db.Roles.ToList();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _db.Roles.ToList();
                return View(model);
            }

            var user = _db.Users.Find(model.Id);
            if (user == null) return NotFound();

            user.Name = model.Name;
            user.Email = model.Email;
            user.RoleId = model.RoleId;

            if (!string.IsNullOrEmpty(model.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null) return NotFound();

            _db.Users.Remove(user);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // READ - Tampilkan profile user yang login
        public IActionResult Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = _db.Users
                .Include(u => u.Role)
                .Include(u => u.Leads) // jika relasi Leads ada
                .Include(u => u.Position)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null) return NotFound();

            return View(user);
        }

        [HttpGet]
        public IActionResult GetUsers(string q)
        {
            var users = _db.Users
                .Where(u => string.IsNullOrEmpty(q) || u.Name != null && u.Name.Contains(q))
                .Select(u => new
                {
                    id = u.Id,
                    text = u.Name ?? u.Email // fallback jika Name null
                })
                .Take(20)
                .ToList();

            return Json(users);
        }


        // GET - Tampilkan form edit profile
        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = _db.Users.Find(userId);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST - Proses simpan perubahan profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(User model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (userId != model.Id)
                return Forbid();

            if (!ModelState.IsValid)
                return View(model);

            var user = _db.Users.Find(userId);
            if (user == null) return NotFound();

            user.Name = model.Name;
            user.Email = model.Email;
            

            _db.SaveChanges();

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Profile));
        }
    }
}
