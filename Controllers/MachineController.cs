using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyAuthDemo.Data;
using MyAuthDemo.Helpers;
using MyAuthDemo.Models;
using MyAuthDemo.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace MyAuthDemo.Controllers
{
    public class MachineController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MachineController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Helper: Get UserId from Claims
        private int? GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out int userId))
                return userId;
            return null;
        }

        // Index (complex: filter, search, paged, include)
        public IActionResult Index(string search = "", int? categoryId = null, MachineCondition? condition = null, MachineStatusType? status = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Machines
                .Include(m => m.Category)
                .Include(m => m.InputByUser)
                .Include(m => m.UpdatedByUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m => m.Name.Contains(search) || m.FullName.Contains(search) || m.AssetCode.Contains(search) || m.Description.Contains(search));
            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(m => m.CategoryId == categoryId);
            if (condition.HasValue)
                query = query.Where(m => m.Condition == condition);
            if (status.HasValue)
                query = query.Where(m => m.StatusType == status);

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var machines = query
                .OrderByDescending(m => m.InputAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MachineViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    FullName = m.FullName,
                    AssetCode = m.AssetCode,
                    Qty = m.Qty,
                    Condition = m.Condition,
                    StatusType = m.StatusType,
                    CategoryName = m.Category != null ? m.Category.Name : "-",
                    ImageUrl = m.ImageUrl,
                    InputAt = m.InputAt,
                    UpdatedAt = m.UpdatedAt,
                    InputByUserName = m.InputByUser != null ? m.InputByUser.Name : "-",
                    UpdatedByUserName = m.UpdatedByUser != null ? m.UpdatedByUser.Name : "-"
                }).ToList();

            var paged = new PagedResult<MachineViewModel>
            {
                Items = machines,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Search = search // tambahkan filter jika perlu
            };

            ViewBag.CategoryOptions = GetCategoryOptions();
            ViewBag.ConditionOptions = EnumHelper.ToSelectList<MachineCondition>();
            ViewBag.StatusTypeOptions = EnumHelper.ToSelectList<MachineStatusType>();
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.Search = search;
            ViewBag.Condition = condition;
            ViewBag.StatusType = status;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(paged); // Fix!
        }


        [HttpGet]
        public IActionResult Create()
        {
            var vm = new MachineViewModel
            {
                Condition = MachineCondition.New,
                StatusType = MachineStatusType.InUseByCustomer,
                CategoryOptions = GetCategoryOptions(),
                ConditionOptions = EnumHelper.ToSelectList<MachineCondition>(),
                StatusTypeOptions = EnumHelper.ToSelectList<MachineStatusType>()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MachineViewModel vm)
        {
            // Pastikan dropdown tetap muncul jika model tidak valid
            vm.CategoryOptions = GetCategoryOptions();
            vm.ConditionOptions = EnumHelper.ToSelectList<MachineCondition>();
            vm.StatusTypeOptions = EnumHelper.ToSelectList<MachineStatusType>();

            if (!ModelState.IsValid) return View(vm);

            // Generate AssetCode: [CategoryNameNoSpace][AutoIncrement]
            var cat = _context.ProductCategories.FirstOrDefault(c => c.Id == vm.CategoryId);
            string prefix = cat != null ? cat.Name.Replace(" ", "") : "MC";
            var existingCodes = _context.Machines
                .Where(m => m.AssetCode.StartsWith(prefix))
                .Select(m => m.AssetCode)
                .ToList();
            string assetCode = AssetCodeHelper.GenerateNextAssetCode(prefix, existingCodes);

            // Upload gambar jika ada
            string? imageUrl = null;
            if (vm.ImageFile != null && vm.ImageFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads/machine");
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(vm.ImageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    vm.ImageFile.CopyTo(fileStream);
                imageUrl = "/uploads/machine/" + fileName;
            }

            // Ambil UserId dari Claims
            int? userId = GetCurrentUserId();

            var machine = new Machine
            {
                CategoryId = vm.CategoryId,
                Name = vm.Name,
                FullName = vm.FullName,
                Description = vm.Description,
                Qty = vm.Qty,
                AssetCode = assetCode,
                ImageUrl = imageUrl,
                Condition = vm.Condition,
                StatusType = vm.StatusType,
                InputByUserId = userId,
                InputAt = DateTime.Now
            };

            _context.Machines.Add(machine);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // EDIT
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var machine = _context.Machines.Include(m => m.Category).FirstOrDefault(m => m.Id == id);
            if (machine == null) return NotFound();

            var vm = new MachineViewModel
            {
                Id = machine.Id,
                CategoryId = machine.CategoryId,
                Name = machine.Name,
                FullName = machine.FullName,
                Description = machine.Description,
                Qty = machine.Qty,
                AssetCode = machine.AssetCode,
                ImageUrl = machine.ImageUrl,
                Condition = machine.Condition,
                StatusType = machine.StatusType,
                InputAt = machine.InputAt,
                UpdatedAt = machine.UpdatedAt
            };
            FillDropdowns(vm);
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(MachineViewModel vm)
        {
            FillDropdowns(vm);
            if (!ModelState.IsValid) return View(vm);

            var machine = _context.Machines.Find(vm.Id);
            if (machine == null) return NotFound();

            machine.CategoryId = vm.CategoryId;
            machine.Name = vm.Name;
            machine.FullName = vm.FullName;
            machine.Description = vm.Description;
            machine.Qty = vm.Qty;
            machine.Condition = vm.Condition;
            machine.StatusType = vm.StatusType;
            machine.UpdatedByUserId = GetCurrentUserId();
            machine.UpdatedAt = DateTime.Now;

            if (vm.RemoveImage && !string.IsNullOrEmpty(machine.ImageUrl))
            {
                var filePath = Path.Combine(_env.WebRootPath, machine.ImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                machine.ImageUrl = null;
            }
            if (vm.ImageFile != null && vm.ImageFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads/machine");
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(vm.ImageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    vm.ImageFile.CopyTo(fileStream);
                machine.ImageUrl = "/uploads/machine/" + fileName;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // DETAILS
        [HttpGet]
        public IActionResult Details(int id)
        {
            var machine = _context.Machines
                .Include(m => m.Category)
                .Include(m => m.InputByUser)
                .Include(m => m.UpdatedByUser)
                .FirstOrDefault(m => m.Id == id);

            if (machine == null) return NotFound();

            var vm = new MachineViewModel
            {
                Id = machine.Id,
                AssetCode = machine.AssetCode,
                Name = machine.Name,
                FullName = machine.FullName,
                Description = machine.Description,
                Qty = machine.Qty,
                CategoryName = machine.Category?.Name ?? "-",
                ImageUrl = machine.ImageUrl,
                Condition = machine.Condition,
                StatusType = machine.StatusType,
                InputByUserName = machine.InputByUser?.Name ?? "-",
                UpdatedByUserName = machine.UpdatedByUser?.Name ?? "-",
                InputAt = machine.InputAt,
                UpdatedAt = machine.UpdatedAt
            };
            return View(vm);
        }



        // DELETE
        [HttpGet]
        // GET: Machine/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var machine = await _context.Machines
                .Where(m => m.Id == id)
                .Select(m => new MachineViewModel
                {
                    Id = m.Id,
                    AssetCode = m.AssetCode,
                    Name = m.Name
                })
                .FirstOrDefaultAsync();
            if (machine == null) return NotFound();
            return View(machine);
        }

        // POST: Machine/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var machine = await _context.Machines.FindAsync(id);
            if (machine == null) return NotFound();

            _context.Machines.Remove(machine);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void FillDropdowns(MachineViewModel vm)
        {
            vm.CategoryOptions = GetCategoryOptions();
            vm.ConditionOptions = EnumHelper.ToSelectList<MachineCondition>();
            vm.StatusTypeOptions = EnumHelper.ToSelectList<MachineStatusType>();
        }
        private List<SelectListItem> GetCategoryOptions()
        {
            return _context.ProductCategories.OrderBy(c => c.Name).Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
        }
    }
}
