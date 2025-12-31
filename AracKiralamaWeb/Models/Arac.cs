using System.ComponentModel.DataAnnotations; // <--- BU SATIR ÖNEMLİ

namespace AracKiralamaWeb.Models
{
    public class Arac
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lütfen araç markasını boş bırakmayınız.")]
        [StringLength(50, ErrorMessage = "Marka adı en fazla 50 karakter olabilir.")]
        public string MarkaModel { get; set; }

        [Required(ErrorMessage = "Plaka zorunludur.")]
        [StringLength(15, ErrorMessage = "Plaka çok uzun.")]
        public string Plaka { get; set; }

        [Range(1, 100000, ErrorMessage = "Günlük fiyat 1 TL ile 100.000 TL arasında olmalıdır.")]
        public double GunlukFiyat { get; set; }

        [Required(ErrorMessage = "Resim URL'si gereklidir.")]
        public string ResimUrl { get; set; }
    }
}