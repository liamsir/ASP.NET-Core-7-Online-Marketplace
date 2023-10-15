using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCWebAppIsmane.Data;
using MVCWebAppIsmane.Models;

namespace MVCWebAppIsmane.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var products = _dataContext.Products.ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            var categories = _dataContext.Categories.ToList(); // Replace with your data retrieval logic.

            ViewBag.Categories = new SelectList(categories, "Id", "Name"); // Populate ViewBag with the list of categories.
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model, IFormFile posterFile)
        {
            if (ModelState.IsValid)
            {
                if (posterFile != null && posterFile.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + posterFile.FileName;
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "pics");
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await posterFile.CopyToAsync(fileStream);
                    }

                    model.Poster = "pics/" + uniqueFileName;
                }

                _dataContext.Products.Add(model);
                _dataContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);        
        }


    }
}
