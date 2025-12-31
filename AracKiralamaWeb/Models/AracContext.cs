using Microsoft.EntityFrameworkCore;

namespace AracKiralamaWeb.Models
{
    public class AracContext : DbContext
    {
        public AracContext(DbContextOptions<AracContext> options) : base(options) { }

        public DbSet<Arac> Araclar { get; set; }
        public DbSet<Kiralama> Kiralamalar { get; set; }
    }
}