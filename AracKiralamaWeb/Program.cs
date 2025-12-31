using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsý
builder.Services.AddDbContext<AracKiralamaWeb.Models.AracContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. GÜVENLÝK (Cookie) AYARLARI
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.LoginPath = "/Auth/Login";       // Giriþ yapýlmadýysa buraya git
        config.LogoutPath = "/Auth/Logout";
        config.Cookie.Name = "AracKiralama.Cerez";
        config.ExpireTimeSpan = TimeSpan.FromDays(1);
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Hata Yönetimi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// ---------------------------------------------------------
// KRÝTÝK NOKTA: Resimlerin (CSS, JS) çalýþmasý için bu ÞART!
app.UseStaticFiles();
// ---------------------------------------------------------

app.UseRouting();

// SIRALAMA ÖNEMLÝ: Önce Kimlik, Sonra Yetki
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();