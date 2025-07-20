using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAuthDemo.Data;
using MyAuthDemo.Models;

namespace MyAuthDemo.Controllers
{
    public class LeadController : Controller
    {
        private readonly AppDbContext _db;

        public LeadController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Lead/Index
        public IActionResult Index(string search = "", string status = "", int page = 1, int pageSize = 10)
        {
            var query = _db.Leads
                .Include(l => l.Group)
                .Include(l => l.User)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(l => l.CompanyName.Contains(search));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(l => l.ContractStatus == status);
            }

            var totalItems = query.Count();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = query
                .OrderBy(l => l.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedResult<Lead>
            {
                Items = items,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalItems = totalItems,
                Search = search,
                Status = status
            };

            ViewBag.StatusOptions = _db.Leads
                .Select(l => l.ContractStatus)
                .Distinct()
                .ToList();

            return View(model);
        }

        // GET: /Lead/Details/5
        public IActionResult Details(int id)
        {
            var lead = _db.Leads
                .Include(l => l.Group)
                .Include(l => l.User)
                .AsNoTracking()
                .FirstOrDefault(l => l.Id == id);

            if (lead == null) return NotFound();

            return View(lead);
        }

        // GET: /Lead/Create
        public IActionResult Create()
        {
            ViewBag.Groups = _db.Groups.ToList();
            return View(new Lead());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Lead model)
        {
            Console.WriteLine("DEBUG ProvinceId: " + model.ProvinceId);
            Console.WriteLine("DEBUG RegencyId: " + model.RegencyId);

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Field {key}: {error.ErrorMessage}");
                    }
                }

                ViewBag.Groups = _db.Groups.ToList();
                TempData["Error"] = "Failed to create lead. Please fix the form errors.";
                return View(model);
            }

            model.UserId = 1; // dummy user
            _db.Leads.Add(model);
            _db.SaveChanges();

            TempData["Success"] = "Lead created successfully.";
            return RedirectToAction(nameof(Index));
        }



        // GET: /Lead/Edit/5
        public IActionResult Edit(int id)
        {
            var lead = _db.Leads.FirstOrDefault(l => l.Id == id);
            if (lead == null)
            {
                return NotFound();
            }

            ViewBag.Groups = _db.Groups.ToList();
            return View(lead);
        }

        // POST: /Lead/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Lead model)
        {
            Console.WriteLine("DEBUG ProvinceId: " + model.ProvinceId);
            Console.WriteLine("DEBUG RegencyId: " + model.RegencyId);

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Field {key}: {error.ErrorMessage}");
                    }
                }

                ViewBag.Groups = _db.Groups.ToList();
                TempData["Error"] = "Update failed. Please fix the form errors.";
                return View(model); // tetap di halaman edit
            }

            var lead = _db.Leads.FirstOrDefault(l => l.Id == model.Id);
            if (lead == null)
            {
                TempData["Error"] = "Lead not found.";
                return RedirectToAction(nameof(Index));
            }

            // Update field
            lead.GroupId = model.GroupId;
            lead.CompanyName = model.CompanyName;
            lead.ProvinceId = model.ProvinceId;
            lead.RegencyId = model.RegencyId;
            lead.Address = model.Address;
            lead.Phone = model.Phone;
            lead.Email = model.Email;
            lead.PICName = model.PICName;
            lead.PICPhone = model.PICPhone;
            lead.PICEmail = model.PICEmail;
            lead.ReferralName = model.ReferralName;

            _db.SaveChanges();

            TempData["Success"] = "Lead updated successfully.";
            return RedirectToAction(nameof(Index));
        }


        // GET: Lead/Delete/5
        public IActionResult Delete(int id)
        {
            var lead = _db.Leads.FirstOrDefault(x => x.Id == id);
            if (lead == null)
            {
                return NotFound();
            }

            return View(lead);
        }

        // POST: Lead/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var lead = _db.Leads.Find(id);
            if (lead == null)
            {
                return NotFound();
            }

            _db.Leads.Remove(lead);
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
