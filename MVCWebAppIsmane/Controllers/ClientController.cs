using Microsoft.AspNetCore.Mvc;
using MVCWebAppIsmane.Models;
using MVCWebAppIsmane.Repositories.IRepositories;

namespace MVCWebAppIsmane.Controllers
{
    public class UserController : Controller
    {

        private readonly IUserRepository _UserRepository;

        public UserController(IUserRepository UserRepository)
        {
            _UserRepository = UserRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        


        

    }
}
