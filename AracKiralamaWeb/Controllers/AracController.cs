using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AracKiralamaWeb.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO; // Dosya işlemleri için

namespace AracKiralamaWeb.Controllers
{
    public class AracController : Controller
    {
        private readonly AracContext _context;

        public AracController(AracContext context)
        {
            _context = context;
        }

        // --- 1. ARAÇ LİSTESİ (ARAMA ÖZELLİKLİ) ---
        public IActionResult Index(string aramaMetni)
        {
            // Veritabanı sorgusunu başlat
            var sorgu = _context.Araclar.AsQueryable();

            // Eğer arama kutusu boş değilse filtrele
            if (!string.IsNullOrEmpty(aramaMetni))
            {
                // Marka/Model VEYA Plaka içinde arama yap (Büyük/küçük harf duyarlı olabilir veritabanına göre)
                sorgu = sorgu.Where(x => x.MarkaModel.Contains(aramaMetni) || x.Plaka.Contains(aramaMetni));
            }

            // Sonuçları listeye çevir ve sayfaya gönder
            return View(sorgu.ToList());
        }

        // --- 2. DASHBOARD (YÖNETİCİ GRAFİK EKRANI) ---
        [Authorize]
        public IActionResult Dashboard()
        {
            var popucarAraclar = _context.Kiralamalar
                .Include(x => x.Arac)
                .GroupBy(x => x.Arac.MarkaModel)
                .Select(g => new {
                    Arac = g.Key,
                    Sayi = g.Count()
                })
                .ToList();

            ViewBag.AracIsimleri = popucarAraclar.Select(x => x.Arac).ToList();
            ViewBag.KiralamaSayilari = popucarAraclar.Select(x => x.Sayi).ToList();

            ViewBag.ToplamKazanc = _context.Kiralamalar.Sum(x => (decimal?)x.ToplamTutar) ?? 0;
            ViewBag.ToplamArac = _context.Araclar.Count();
            ViewBag.ToplamKiralama = _context.Kiralamalar.Count();

            return View();
        }

        // --- 3. YENİ ARAÇ EKLEME SAYFASI (GET) ---
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // --- 4. YENİ ARAÇ KAYDETME (POST) - [RESİM YÜKLEMELİ] ---
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Arac yeniArac, IFormFile? resimDosyasi)
        {
            if (ModelState.IsValid)
            {
                if (resimDosyasi != null)
                {
                    string uzanti = Path.GetExtension(resimDosyasi.FileName);
                    string yeniResimAdi = Guid.NewGuid().ToString() + uzanti;
                    string kayitYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", yeniResimAdi);

                    using (var stream = new FileStream(kayitYolu, FileMode.Create))
                    {
                        await resimDosyasi.CopyToAsync(stream);
                    }
                    yeniArac.ResimUrl = yeniResimAdi;
                }
                else
                {
                    yeniArac.ResimUrl = "default-car.jpg";
                }

                _context.Araclar.Add(yeniArac);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(yeniArac);
        }

        // --- 5. DÜZENLEME SAYFASI (GET) ---
        [Authorize]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var arac = _context.Araclar.Find(id);
            if (arac == null) return NotFound();
            return View(arac);
        }

        // --- 6. DÜZENLEME İŞLEMİ (POST) ---
        [Authorize]
        [HttpPost]
        public IActionResult Edit(Arac guncelArac)
        {
            if (ModelState.IsValid)
            {
                _context.Araclar.Update(guncelArac);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(guncelArac);
        }

        // --- 7. SİLME İŞLEMİ ---
        [Authorize]
        public IActionResult Delete(int id)
        {
            var arac = _context.Araclar.Find(id);
            if (arac != null)
            {
                _context.Araclar.Remove(arac);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // --- 8. KİRALAMA SAYFASI GÖSTER (GET) ---
        [HttpGet]
        public IActionResult Kirala(string plaka)
        {
            var secilenArac = _context.Araclar.FirstOrDefault(x => x.Plaka == plaka);
            if (secilenArac == null) return RedirectToAction("Index");
            return View(secilenArac);
        }

        // --- 9. KİRALAMA İŞLEMİNİ YAP (POST) ---
        [HttpPost]
        public IActionResult Kirala(Kiralama yeniKiralama, string Plaka)
        {
            var arac = _context.Araclar.FirstOrDefault(x => x.Plaka == Plaka);
            if (arac == null) return RedirectToAction("Index");

            var cakismaVarMi = _context.Kiralamalar.Any(k =>
                k.AracId == arac.Id &&
                (
                    (yeniKiralama.BaslangicTarihi >= k.BaslangicTarihi && yeniKiralama.BaslangicTarihi <= k.BitisTarihi) ||
                    (yeniKiralama.BitisTarihi >= k.BaslangicTarihi && yeniKiralama.BitisTarihi <= k.BitisTarihi) ||
                    (yeniKiralama.BaslangicTarihi <= k.BaslangicTarihi && yeniKiralama.BitisTarihi >= k.BitisTarihi)
                )
            );

            if (cakismaVarMi)
            {
                TempData["Hata"] = "❌ Seçtiğiniz tarihlerde araç maalesef dolu.";
                return View(arac);
            }

            TimeSpan gunFarki = yeniKiralama.BitisTarihi - yeniKiralama.BaslangicTarihi;
            int gunSayisi = (int)gunFarki.TotalDays;
            if (gunSayisi <= 0) gunSayisi = 0;

            yeniKiralama.AracId = arac.Id;
            yeniKiralama.ToplamTutar = gunSayisi * arac.GunlukFiyat;

            _context.Kiralamalar.Add(yeniKiralama);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // --- 10. ADMİN KİRALAMA LİSTESİ ---
        [Authorize]
        public IActionResult Kiralamalar()
        {
            var liste = _context.Kiralamalar
                .Include(k => k.Arac)
                .OrderByDescending(x => x.Id)
                .ToList();
            return View(liste);
        }

        // --- 11. KİRALAMA İPTALİ ---
        [Authorize]
        public IActionResult KiralamaIptal(int id)
        {
            var kiralama = _context.Kiralamalar.Find(id);
            if (kiralama != null)
            {
                _context.Kiralamalar.Remove(kiralama);
                _context.SaveChanges();
            }
            return RedirectToAction("Kiralamalar");
        }
    }
}