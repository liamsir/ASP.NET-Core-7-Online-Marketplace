using MVCWebAppIsmane.Data;
using MVCWebAppIsmane.Models;
using MVCWebAppIsmane.Repositories.IRepositories;
using System;

namespace MVCWebAppIsmane.Repositories
{
    public class LigneAchatRepository : Repository<LigneAchat> , ILigneAchatRepository
    {
        public LigneAchatRepository(DataContext context) : base(context)
        {
            
        }       
    }
}
