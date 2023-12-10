using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCWebAppIsmane.Data;
using MVCWebAppIsmane.Repositories;

namespace MVCWebAppIsmane.Controllers
{
    public class CategoryController : Controller
    {

        private readonly CategoryRepository _categoryRepository;

        public CategoryController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
            var categories = _categoryRepository.GetAll();
            return View(categories);
        }

    }
}
