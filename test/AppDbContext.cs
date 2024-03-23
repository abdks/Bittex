using Microsoft.EntityFrameworkCore;

namespace test
{
    public class AppDbContext : DbContext
    {
        public DbSet<CryptoCurrency> CryptoCurrencies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=ABDULLAH;Initial Catalog=Crypto;Integrated Security=True;TrustServerCertificate=True");
        }
    }
}
