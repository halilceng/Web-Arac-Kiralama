using AracKiralamaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AracKiralamaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Ana Sayfa
        public IActionResult Index()
        {
            return View();
        }

        // Hakkýmýzda Sayfasý
        public IActionResult Hakkimizda()
        {
            return View();
        }

        // Ýletiþim Sayfasý
        public IActionResult Iletisim()
        {
            return View();
        }

        // --- YENÝ EKLEDÝÐÝMÝZ SSS SAYFASI ---
        public IActionResult SSS()
        {
            return View();
        }

        // Gizlilik Politikasý (Varsayýlan)
        public IActionResult Privacy()
        {
            return View();
        }

        // Hata Yönetimi
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}