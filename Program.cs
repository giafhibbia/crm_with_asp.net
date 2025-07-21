using MyAuthDemo.Data;
using MyAuthDemo.Repositories;
using MyAuthDemo.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using MyAuthDemo.Data.Seeders;
using MyAuthDemo.Models;

var builder = WebApplication.CreateBuilder(args);

// Registrasi service dan dbcontext
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);



// Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PermissionService>();

// Autentikasi & Autorisasi
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
    });

builder.Services.AddAuthorization();

// Firebase config opsional
builder.Services.Configure<FirebaseOptions>(
    builder.Configuration.GetSection("Firebase"));

// Build app
var app = builder.Build();

// Seeder hanya dijalankan tanpa migrasi
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedProvinceData.Initialize(dbContext);
    SeedRegencyData.Initialize(dbContext);
    SeedPositionData.Initialize(dbContext);
    SeedAdminUserData.Initialize(dbContext);
    SeedRolePermissionData.Initialize(dbContext);
}

// Middleware pipeline
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

// Routing utama
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}"
);

app.MapControllers();

app.Run();
