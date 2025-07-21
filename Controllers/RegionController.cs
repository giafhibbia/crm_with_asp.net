using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAuthDemo.Data;
using System.Linq;

namespace MyAuthDemo.Controllers
{
    [Route("region")]
    public class RegionController : Controller
    {
        private readonly AppDbContext _context;

        public RegionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("provinces")]
        public IActionResult GetProvinces(string? q)
        {
            Console.WriteLine("QUERY = " + q);

            var queryable = _context.Provinces.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                queryable = queryable
                    .Where(p => p.Name.ToLower().Contains(q.ToLower()));
            }

            var result = queryable
                .OrderBy(p => p.Name)
                .Select(p => new { id = p.Id, text = p.Name })
                .ToList();

            return Json(result);
        }


        [HttpGet("regencies")]
        public IActionResult GetRegencies(int provinceId, string? term)
        {
            var query = _context.Regencies
                .Where(r => r.ProvinceId == provinceId);

            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(r => r.Name.Contains(term));
            }

            var result = query
                .OrderBy(r => r.Name)
                .Select(r => new { id = r.Id, text = r.Name })
                .ToList();

            return Json(result);
        }

    }
}
