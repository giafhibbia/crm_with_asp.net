using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAuthDemo.Data;
using MyAuthDemo.Services;
using MyAuthDemo.Models;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

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

        // READ - Tampilkan profile user yang login
        public IActionResult Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = _db.Users.Find(userId);
            if (user == null) return NotFound();
            return View(user);
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
