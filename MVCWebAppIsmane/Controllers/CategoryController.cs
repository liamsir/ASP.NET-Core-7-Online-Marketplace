using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCWebAppIsmane.Data;

namespace MVCWebAppIsmane.Controllers
{
    public class CategoryController : Controller
    {

        private readonly DataContext _dataContext;

        public CategoryController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            var categories = _dataContext.Categories.ToList();
            return View(categories);
        }

    }
}
