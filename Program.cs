using MyAuthDemo.Data;
using MyAuthDemo.Repositories;
using MyAuthDemo.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using MyAuthDemo.Data.Seeders;
using MyAuthDemo.Models;

var builder = WebApplication.CreateBuilder(args);

// Registrasi service dan dbcontext seperti biasa
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

builder.Services.AddDbContext<RegionDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        new MySqlServerVersion(new Version(10, 4, 28)))); // sesuaikan versi MariaDB kamu

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
    });
builder.Services.AddAuthorization();

builder.Services.Configure<FirebaseOptions>(
    builder.Configuration.GetSection("Firebase"));


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var connection = dbContext.Database.GetDbConnection();
    connection.Open();

    using (var command = connection.CreateCommand())
    {
        // Cek apakah kolom ProvinceId sudah ada di tabel Leads
        command.CommandText = @"
            SELECT COUNT(*) 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = 'Leads' AND COLUMN_NAME = 'ProvinceId';
        ";

        var exists = Convert.ToInt32(command.ExecuteScalar());

        if (exists == 0)
        {
            Console.WriteLine("Kolom ProvinceId belum ada, jalankan migrasi...");
            dbContext.Database.Migrate();
        }
        else
        {
            Console.WriteLine("Kolom ProvinceId sudah ada, lewati migrasi agar tidak error.");
        }
    }

    // Jalankan seeder seperti biasa
    SeedData.Initialize(dbContext);

    var regionDb = scope.ServiceProvider.GetRequiredService<RegionDbContext>();
    SeedProvinceData.Initialize(regionDb);
    SeedRegencyData.Initialize(regionDb);
}



// Middleware pipeline config (error handling, static files, routing, dll)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Map controller route MVC biasa
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}"
);

// Map attribute routed controllers (API controller seperti SeedController)
app.MapControllers();

app.Run();