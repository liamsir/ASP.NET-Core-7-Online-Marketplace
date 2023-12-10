
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVCWebAppIsmane.Models;
using MVCWebAppIsmane.Repositories.IRepositories;
using Newtonsoft.Json;



namespace MVCWebAppIsmane.Controllers
{
    public class ProductController : Controller
    {
        //private readonly DataContext _dataContext;
        private readonly ILogger<ProductController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ILogger<ProductController> logger, ICategoryRepository categoryRepository , IProductRepository productRepository, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAll();
            return View(products);
        }


        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAll(); // Assuming GetAll() retrieves all categories
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
                    model.Poster = "~/pics/" + uniqueFileName;
                }

                _productRepository.Create(model);
                return RedirectToAction("Index");

                //whateer you want
            }

            return View(model);        
        }




        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAll();

            var product = await _productRepository.GetById(Id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product model, IFormFile posterFile)
        {
            if (ModelState.IsValid)
            {
                Product? existingProduct = await _productRepository.GetById(model.Id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                if (posterFile != null && posterFile.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + posterFile.FileName;
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "pics");
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await posterFile.CopyToAsync(fileStream);
                    }

                    if (!string.IsNullOrEmpty(existingProduct.Poster))
                    {
                        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduct.Poster);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    model.Poster = "~/pics/" + uniqueFileName;
                }
                else
                {
                    model.Poster = existingProduct.Poster;
                }

                existingProduct.Name = model.Name;
                existingProduct.Description = model.Description;
                existingProduct.Price = model.Price;
                existingProduct.IdCategory = model.IdCategory;
                existingProduct.Poster = model.Poster;

                await _productRepository.Update(existingProduct);

                return RedirectToAction("Index");
            }

            // Handle invalid model state here
            return View(model);
        }



        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            // Retrieve the cart items from cookies
            List<int> cartItems = new List<int>();
            var existingCart = Request.Cookies["CartItems"];
            if (!string.IsNullOrEmpty(existingCart))
            {
                try
                {
                    cartItems = JsonConvert.DeserializeObject<List<int>>(existingCart);

                }
                catch (JsonReaderException ex)
                {
                    // Handle the exception or log it
                    // Example: Log the exception message
                    Console.WriteLine("Error deserializing JSON: " + ex.Message);
                }
                
            }

            foreach (int item in cartItems)
            {
                _logger.LogInformation("=================>\t" + item);
            }

            // Add the selected product to the cart
            try
            {
                cartItems.Add(productId);
            }
            catch (JsonReaderException ex) { Console.WriteLine("Error deserializing JSON: " + ex.Message); }

            // Save the updated cart items in cookies
            string cartJson = JsonConvert.SerializeObject(cartItems);
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7) // Adjust the expiration time as needed
            };
            Response.Cookies.Append("CartItems", cartJson, option);

            // Redirect or return a response as needed
            return RedirectToAction("Index", "Product");
        }


        public async Task<IActionResult> ViewCart()
        {
            var existingCart = Request.Cookies["CartItems"];

            List<int> cartItems = new List<int>();
            if (!string.IsNullOrEmpty(existingCart))
            {
                try
                {
                    cartItems = JsonConvert.DeserializeObject<List<int>>(existingCart);
                }
                catch (JsonReaderException ex)
                {
                    // Handle the exception or log it
                    // Example: Log the exception message
                    Console.WriteLine("Error deserializing JSON: " + ex.Message);
                }
            }

            // Use the cart items to retrieve products from your repository
            var productInCart = await _productRepository.Get(p => cartItems.Contains(p.Id));
            return View("Index", productInCart);
        }



        public async Task<IActionResult> SearchByCategory(int categoryId, string searchTerm)
        {
            IEnumerable<Product> products;

            if (categoryId == 0) // Assuming 0 represents "Choose category"
            {
                // Display all products if no specific category is selected
                products = await _productRepository.GetAll();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    // Search only by category when searchTerm is empty
                    products = await _productRepository.Get(p => p.IdCategory == categoryId);
                }
                else
                {
                    // Search by category and name when searchTerm is provided
                    products = await _productRepository.Get(p => p.IdCategory == categoryId && p.Name.Contains(searchTerm));
                }
            }

            return View("Index", products);
        }

    }
}
