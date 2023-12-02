using MVCWebAppIsmane.Data;
using MVCWebAppIsmane.Models;
using System;

namespace MVCWebAppIsmane.Repositories
{
    public class LigneAchatRepository : Repository<Category>
    {
        public LigneAchatRepository(DataContext context) : base(context)
        {
            
        }       
    }
}
