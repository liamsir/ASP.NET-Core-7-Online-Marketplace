using Microsoft.AspNetCore.Mvc;

namespace MVCWebAppIsmane.Controllers
{
    public class ClientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
