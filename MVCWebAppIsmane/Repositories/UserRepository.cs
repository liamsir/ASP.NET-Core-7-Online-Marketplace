using Microsoft.EntityFrameworkCore;
using MVCWebAppIsmane.Data;
using MVCWebAppIsmane.Models;
using MVCWebAppIsmane.Repositories.IRepositories;
using System;

namespace MVCWebAppIsmane.Repositories
{
    public class UserRepository : Repository<User> , IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {

        }

       
    }
}
