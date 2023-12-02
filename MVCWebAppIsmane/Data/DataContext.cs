using Microsoft.EntityFrameworkCore;
using MVCWebAppIsmane.Models;
namespace MVCWebAppIsmane.Data
{
    public class DataContext : DbContext
    {       
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Achat> Achats { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<LigneAchat> LigneAchats { get; set; }


    }
}

