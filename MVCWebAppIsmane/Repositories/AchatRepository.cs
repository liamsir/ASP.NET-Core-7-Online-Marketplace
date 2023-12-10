using MVCWebAppIsmane.Data;
using MVCWebAppIsmane.Models;
using MVCWebAppIsmane.Repositories.IRepositories;
using System;

namespace MVCWebAppIsmane.Repositories
{
    public class AchatRepository : Repository<Achat> , IAchatRepository
    {
        public AchatRepository(DataContext context) : base(context)
        {
        }
        
    }
}
