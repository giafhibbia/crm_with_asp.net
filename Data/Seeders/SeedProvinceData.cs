using System.Collections.Generic;
using System.Linq;
using MyAuthDemo.Data;
using MyAuthDemo.Models.Region;

namespace MyAuthDemo.Data.Seeders
{
    public static class SeedProvinceData
    {
        public static void Initialize(AppDbContext context)
        {
            try
            {
                Console.WriteLine("🟡 SeedProvinceData: Memulai inisialisasi...");

                if (context == null)
                {
                    Console.WriteLine("❌ RegionDbContext is null");
                    return;
                }

                if (context.Provinces == null)
                {
                    Console.WriteLine("❌ DbSet<Province> is null");
                    return;
                }

                Console.WriteLine("🔍 Mengecek apakah sudah ada data provinsi...");
                if (context.Provinces.Any())
                {
                    Console.WriteLine("✅ Data provinsi sudah ada, skip seeding.");
                    return;
                }

                Console.WriteLine("➕ Menambahkan data provinsi...");
                var provinces = new List<Province>
                {
                new Province { Id = 11, Name = "ACEH" },
                new Province { Id = 12, Name = "SUMATERA UTARA" },
                new Province { Id = 13, Name = "SUMATERA BARAT" },
                new Province { Id = 14, Name = "RIAU" },
                new Province { Id = 15, Name = "JAMBI" },
                new Province { Id = 16, Name = "SUMATERA SELATAN" },
                new Province { Id = 17, Name = "BENGKULU" },
                new Province { Id = 18, Name = "LAMPUNG" },
                new Province { Id = 19, Name = "KEPULAUAN BANGKA BELITUNG" },
                new Province { Id = 21, Name = "KEPULAUAN RIAU" },
                new Province { Id = 31, Name = "DKI JAKARTA" },
                new Province { Id = 32, Name = "JAWA BARAT" },
                new Province { Id = 33, Name = "JAWA TENGAH" },
                new Province { Id = 34, Name = "DAERAH ISTIMEWA YOGYAKARTA" },
                new Province { Id = 35, Name = "JAWA TIMUR" },
                new Province { Id = 36, Name = "BANTEN" },
                new Province { Id = 51, Name = "BALI" },
                new Province { Id = 52, Name = "NUSA TENGGARA BARAT" },
                new Province { Id = 53, Name = "NUSA TENGGARA TIMUR" },
                new Province { Id = 61, Name = "KALIMANTAN BARAT" },
                new Province { Id = 62, Name = "KALIMANTAN TENGAH" },
                new Province { Id = 63, Name = "KALIMANTAN SELATAN" },
                new Province { Id = 64, Name = "KALIMANTAN TIMUR" },
                new Province { Id = 65, Name = "KALIMANTAN UTARA" },
                new Province { Id = 71, Name = "SULAWESI UTARA" },
                new Province { Id = 72, Name = "SULAWESI TENGAH" },
                new Province { Id = 73, Name = "SULAWESI SELATAN" },
                new Province { Id = 74, Name = "SULAWESI TENGGARA" },
                new Province { Id = 75, Name = "GORONTALO" },
                new Province { Id = 76, Name = "SULAWESI BARAT" },
                new Province { Id = 81, Name = "MALUKU" },
                new Province { Id = 82, Name = "MALUKU UTARA" },
                new Province { Id = 91, Name = "PAPUA" },
                new Province { Id = 92, Name = "PAPUA BARAT" },
                new Province { Id = 93, Name = "PAPUA SELATAN" },
                new Province { Id = 94, Name = "PAPUA TENGAH" },
                new Province { Id = 95, Name = "PAPUA PEGUNUNGAN" }
            };

        context.Provinces.AddRange(provinces);
        context.SaveChanges();

        Console.WriteLine("✅ SeedProvinceData selesai.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Exception saat seed province:");
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
        throw; // lempar ulang supaya kamu bisa lihat error lengkap di konsol
    }
}
    }
}
