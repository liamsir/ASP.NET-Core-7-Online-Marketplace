using Microsoft.EntityFrameworkCore;
using MVCWebAppIsmane.Data;
using MVCWebAppIsmane.Models;
using MVCWebAppIsmane.Repositories.IRepositories;
using System;

namespace MVCWebAppIsmane.Repositories
{
    public class ClientRepository : Repository<Client> , IClientRepository
    {
        public ClientRepository(DataContext context) : base(context)
        {

        }

       
    }
}
