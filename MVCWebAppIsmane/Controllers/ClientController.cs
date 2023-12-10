using Microsoft.AspNetCore.Mvc;
using MVCWebAppIsmane.Models;
using MVCWebAppIsmane.Repositories.IRepositories;

namespace MVCWebAppIsmane.Controllers
{
    public class ClientController : Controller
    {

        private readonly IClientRepository _clientRepository;

        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        


        

    }
}
