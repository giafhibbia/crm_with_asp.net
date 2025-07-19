using Microsoft.EntityFrameworkCore;
using MyAuthDemo.Models.Region;

namespace MyAuthDemo.Data
{
    public class RegionDbContext : DbContext
    {
        public RegionDbContext(DbContextOptions<RegionDbContext> options)
            : base(options)
        {
        }

        public DbSet<Province> Provinces { get; set; } = null!;
        public DbSet<Regency> Regencies { get; set; } = null!;

    }
}
