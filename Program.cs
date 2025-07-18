// Import semua dependency utama
using MyAuthDemo.Data;
using MyAuthDemo.Repositories;
using MyAuthDemo.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ========== SERVICE REGISTRATION ==========

// 1. Aktifkan support MVC (Controller + Razor Views)
builder.Services.AddControllersWithViews();

// 2. Daftarkan AppDbContext (Entity Framework Core) untuk koneksi MySQL
//    Ini membuat .NET bisa query/insert data ke database.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// 3. Daftarkan Repository dan Service ke DI Container
//    Supaya bisa di-inject otomatis di Controller/Service lain.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AuthService>();

// 4. Daftarkan Cookie Authentication
//    Untuk fitur login, session, dan proteksi halaman (Authorize)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Redirect ke sini kalau belum login
        options.AccessDeniedPath = "/Auth/Login";
    });

// 5. Daftarkan Authorization Middleware (wajib kalau pakai [Authorize] di controller)
builder.Services.AddAuthorization();

// ========== APP PIPELINE CONFIGURATION ==========

var app = builder.Build();

// 6. Error handling & HSTS (default template, optional, aman dibiarkan)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 7. Redirect HTTP ke HTTPS (default, demi keamanan)
app.UseHttpsRedirection();

// 8. Aktifkan serving file static (CSS, JS, gambar di wwwroot)
app.UseStaticFiles();

// 9. Routing system ASP.NET MVC
app.UseRouting();

// 10. Aktifkan Authentication (HARUS sebelum Authorization)
app.UseAuthentication();

// 11. Aktifkan Authorization (agar [Authorize] attribute bekerja)
app.UseAuthorization();

// 12. Definisikan routing default (misal: / akan ke /Auth/Login)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}"
);

// 13. Jalankan aplikasi
app.Run();
