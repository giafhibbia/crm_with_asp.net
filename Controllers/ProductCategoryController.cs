using Microsoft.AspNetCore.Mvc;
using MyAuthDemo.Data;
using MyAuthDemo.Models;
using MyAuthDemo.ViewModels;
using System.IO;
using ClosedXML.Excel;
namespace MyAuthDemo.Controllers
{
    public class ProductCategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductCategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int page = 1, int pageSize = 10, string search = "")
        {
            var query = _context.ProductCategories.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Name.Contains(search));

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var categories = query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResult<ProductCategory>
            {
                Items = categories,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Search = search
            };

            return View(result); // <--- SUDAH SESUAI VIEW
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var cat = _context.ProductCategories.Find(id);
            if (cat == null) return NotFound();
            return View(cat);
        }


        // CREATE
        [HttpGet]
        public IActionResult Create() => View(new ProductCategoryViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCategoryViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            string? iconUrl = null;

            // Handle icon upload jika ada
            if (vm.IconFile != null && vm.IconFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads/category-icons");
                Directory.CreateDirectory(uploads); // buat folder jika belum ada
                var fileName = Guid.NewGuid() + Path.GetExtension(vm.IconFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    vm.IconFile.CopyTo(fileStream);
                }
                iconUrl = "/uploads/category-icons/" + fileName;
            }

            var category = new ProductCategory
            {
                Name = vm.Name,
                Description = vm.Description,
                IconUrl = iconUrl
            };

            _context.ProductCategories.Add(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // EDIT
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.ProductCategories.Find(id);
            if (category == null) return NotFound();

            var vm = new ProductCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IconUrl = category.IconUrl
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(ProductCategoryViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var category = _context.ProductCategories.Find(vm.Id);
            if (category == null) return NotFound();

            category.Name = vm.Name;
            category.Description = vm.Description;
            category.UpdatedAt = DateTime.Now;

            // Hapus icon jika dicentang
            if (vm.RemoveIcon && !string.IsNullOrEmpty(category.IconUrl))
            {
                // Path ke file fisik
                var iconPath = Path.Combine(_env.WebRootPath, category.IconUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(iconPath))
                {
                    System.IO.File.Delete(iconPath);
                }
                category.IconUrl = null;
            }

            // Upload icon baru (jika diisi)
            if (vm.IconFile != null && vm.IconFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads/category-icons");
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(vm.IconFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    vm.IconFile.CopyTo(fileStream);
                }
                category.IconUrl = "/uploads/category-icons/" + fileName;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: /ProductCategory/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = _context.ProductCategories.Find(id);
            if (category == null) return NotFound();

            return View(category); // Tampilkan konfirmasi delete
        }

        [HttpPost]
        public IActionResult BulkDelete(int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                TempData["Error"] = "Tidak ada kategori yang dipilih.";
                return RedirectToAction("Index");
            }

            var itemsToDelete = _context.ProductCategories.Where(x => ids.Contains(x.Id)).ToList();
            if (itemsToDelete.Any())
            {
                _context.ProductCategories.RemoveRange(itemsToDelete);
                _context.SaveChanges();
                TempData["Success"] = $"{itemsToDelete.Count} kategori berhasil dihapus.";
            }
            else
            {
                TempData["Error"] = "Data tidak ditemukan.";
            }

            return RedirectToAction("Index");
        }

        // POST: /ProductCategory/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _context.ProductCategories.Find(id);
            if (category == null) return NotFound();

            _context.ProductCategories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ExportExcel()
        {
            var categories = _context.ProductCategories
                .OrderBy(c => c.Id)
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Product Categories");

                // Title
                worksheet.Cell("A1").Value = "Product Categories";
                worksheet.Range("A1:C1").Merge().Style
                    .Font.SetBold()
                    .Font.FontSize = 16;
                worksheet.Row(1).Height = 28;

                // Header
                worksheet.Cell("A2").Value = "No";
                worksheet.Cell("B2").Value = "Category Name";
                worksheet.Cell("C2").Value = "Description";
                worksheet.Range("A2:C2").Style.Font.Bold = true;
                worksheet.Range("A2:C2").Style.Fill.BackgroundColor = XLColor.LightGreen;

                // Data
                int row = 3;
                int no = 1;
                foreach (var cat in categories)
                {
                    worksheet.Cell(row, 1).Value = no++;
                    worksheet.Cell(row, 2).Value = cat.Name;
                    worksheet.Cell(row, 3).Value = cat.Description;
                    row++;
                }

                // Auto size columns
                worksheet.Columns().AdjustToContents();

                // Download
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    var fileName = $"product_categories_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                fileName);
                }
            }
        }

        [HttpPost]
        public IActionResult ImportExcel(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                TempData["Error"] = "File not selected.";
                return RedirectToAction(nameof(Index));
            }

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheets.First();
                    var rows = worksheet.RowsUsed().Skip(1); // skip header

                    foreach (var row in rows)
                    {
                        var name = row.Cell(2).GetString();
                        var desc = row.Cell(3).GetString();

                        // Cek jika name kosong, skip row
                        if (string.IsNullOrWhiteSpace(name)) continue;

                        // Optional: Cek duplikat
                        if (_context.ProductCategories.Any(c => c.Name == name)) continue;

                        var cat = new ProductCategory
                        {
                            Name = name,
                            Description = desc
                        };
                        _context.ProductCategories.Add(cat);
                    }
                    _context.SaveChanges();
                }
            }

            TempData["Success"] = "Import berhasil!";
            return RedirectToAction(nameof(Index));
        }


        
    }
    
}
