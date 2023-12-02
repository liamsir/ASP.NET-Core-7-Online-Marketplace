using MVCWebAppIsmane.Data;
using MVCWebAppIsmane.Models;
using System;

namespace MVCWebAppIsmane.Repositories
{
    public class AchatRepository : Repository<Category>
    {
        public AchatRepository(DataContext context) : base(context)
        {
        }
        
    }
}
