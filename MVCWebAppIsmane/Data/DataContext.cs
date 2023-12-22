using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MVCWebAppIsmane.Models;
using System;

namespace MVCWebAppIsmane.Data
{
    public class DataContext : IdentityDbContext<User>
    {       
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Achat> Achats { get; set; }
        public DbSet<User> Clients { get; set; }
        public DbSet<LigneAchat> LigneAchats { get; set; }


    }
}

