using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AracKiralamaWeb.Controllers
{
    public class AuthController : Controller
    {
        // 1. Giriş Sayfasını Aç (GET)
        [HttpGet]
        public IActionResult Login()
        {
            // Eğer zaten giriş yapmışsa, tekrar login sayfasına girmesin, panele gitsin
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Arac");
            }
            return View();
        }

        // 2. Giriş İşlemini Yap (POST)
        [HttpPost]
        public async Task<IActionResult> Login(string kadi, string sifre)
        {
            // ÖRNEK: Sabit kullanıcı adı ve şifre (Bunu veritabanından da çekebilirsin)
            // Kullanıcı Adı: admin
            // Şifre: 123
            if (kadi == "admin" && sifre == "123")
            {
                // A) Kullanıcı bilgilerini (Claim) hazırla
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, kadi),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                // B) Kimlik Kartını (Identity) oluştur
                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                // C) Sisteme Giriş Yap (Çerezi Tarayıcıya Göm)
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

                // D) Başarılıysa Araç Listesine git
                return RedirectToAction("Index", "Arac");
            }

            // Hatalıysa uyarı ver
            ViewBag.Hata = "Kullanıcı adı veya şifre yanlış!";
            return View();
        }

        // --- SENİN İSTEDİĞİN ÇIKIŞ (LOGOUT) KISMI ---
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth"); // Çerezi siler (Oturumu kapatır)
            return RedirectToAction("Index", "Home");     // Ana sayfaya yönlendirir
        }
    }
}