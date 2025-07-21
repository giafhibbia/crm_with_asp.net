using System.Collections.Generic;
using System.Linq;
using MyAuthDemo.Data;
using MyAuthDemo.Models;

namespace MyAuthDemo.Data.Seeders
{
    public static class SeedPositionData
    {
        public static void Initialize(AppDbContext context)
        {
            if (context.Positions.Any())
            {
                Console.WriteLine("✅ Position data already seeded.");
                return;
            }

            var positions = new List<Position>
            {
                new Position { Name = "Admin" },
                new Position { Name = "Manager" },
                new Position { Name = "Supervisor" },
                new Position { Name = "Staff" },
                new Position { Name = "Developer" },
                new Position { Name = "Sales" }
            };

            context.Positions.AddRange(positions);
            context.SaveChanges();

            Console.WriteLine("✅ Seeded default positions.");
        }
    }
}
