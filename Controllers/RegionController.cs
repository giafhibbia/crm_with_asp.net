using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAuthDemo.Data;
using System.Linq;

namespace MyAuthDemo.Controllers
{
    [Route("region")]
    public class RegionController : Controller
    {
        private readonly RegionDbContext _context;

        public RegionController(RegionDbContext context)
        {
            _context = context;
        }

        [HttpGet("provinces")]
        public IActionResult GetProvinces()
        {
            var provinces = _context.Provinces
                .Select(p => new { id = p.Id, text = p.Name })
                .ToList();
            return Json(provinces);
        }

        [HttpGet("regencies")]
        public IActionResult GetRegencies(int provinceId)
        {
            var regencies = _context.Regencies
                .Where(r => r.ProvinceId == provinceId)
                .Select(r => new { id = r.Id, text = r.Name })
                .ToList();
            return Json(regencies);
        }
    }
}
