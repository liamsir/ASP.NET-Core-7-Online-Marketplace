using MVCWebAppIsmane.Data;
using MVCWebAppIsmane.Models;
using MVCWebAppIsmane.Repositories.IRepositories;
using System;

namespace MVCWebAppIsmane.Repositories
{
    public class CategoryRepository : Repository<Category> , ICategoryRepository
    {
        public CategoryRepository(DataContext context) : base(context)
        {
        }

        
    }
}
