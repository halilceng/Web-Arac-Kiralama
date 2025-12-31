using System.ComponentModel.DataAnnotations;

namespace AracKiralamaWeb.Models
{
    public class Kiralama
    {
        public int Id { get; set; }

        public int AracId { get; set; } // Hangi araç? (Arac tablosundaki Id)

        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }

        public double ToplamTutar { get; set; }

        public virtual Arac Arac { get; set; }
    }
}