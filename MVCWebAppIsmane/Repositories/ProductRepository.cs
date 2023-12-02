using MVCWebAppIsmane.Data;
using MVCWebAppIsmane.Models;
using System;

namespace MVCWebAppIsmane.Repositories
{
    public class ProductRepository : Repository<Category>
    {
        public ProductRepository(DataContext context) : base(context)
        {
        }       
    }
}
