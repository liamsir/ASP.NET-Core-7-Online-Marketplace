using MVCWebAppIsmane.Data;
using MVCWebAppIsmane.Models;
using System;

namespace MVCWebAppIsmane.Repositories
{
    public class ClientRepository : Repository<Category>
    {
        public ClientRepository(DataContext context) : base(context)
        {

        }        
    }
}
