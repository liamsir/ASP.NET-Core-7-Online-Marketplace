using MVCWebAppIsmane.Data;
using MVCWebAppIsmane.Models;
using System;

namespace MVCWebAppIsmane.Repositories
{
    public class CategoryRepository : Repository<Category>
    {
        public CategoryRepository(DataContext context) : base(context)
        {
        }

        
    }
}
