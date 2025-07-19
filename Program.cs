using MyAuthDemo.Data;
using MyAuthDemo.Repositories;
using MyAuthDemo.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using MyAuthDemo.Data.Seeders;

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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialize(dbContext); // ← kalau kamu punya seed ini juga

    var regionDb = scope.ServiceProvider.GetRequiredService<RegionDbContext>();
    SeedProvinceData.Initialize(regionDb); // ← baris yang error kemarin
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