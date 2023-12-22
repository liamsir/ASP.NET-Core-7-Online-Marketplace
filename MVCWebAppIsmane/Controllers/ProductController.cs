
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using MVCWebAppIsmane.Models;
using MVCWebAppIsmane.Repositories.IRepositories;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MVCWebAppIsmane.Controllers
{
    public class ProductController : Controller
    {
        
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ProductController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IMemoryCache memoryCache, ILogger<ProductController> logger, ICategoryRepository categoryRepository , IProductRepository productRepository, IWebHostEnvironment webHostEnvironment)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
        }



        public async Task<IActionResult> Index()
        {
            _logger.LogInformation($"==============> Open the app at {DateTime.UtcNow}");
            bool exist = _memoryCache.TryGetValue("Products", out IEnumerable<Product> products);
            if (!exist)
            {
                _logger.LogInformation("============> Products not found in cache. Fetching from repository...");

                products = await _productRepository.GetAll();

                if (products != null && products.Any())
                {
                    _memoryCache.Set("Products", products, TimeSpan.FromMinutes(10)); // Cache for 10 minutes                
                    _logger.LogInformation("=========> Products stored in cache.");
                }
                else
                {
                    _logger.LogWarning("===================> Products retrieved from repository are null or empty.");
                }
            }
            else
            {
                _logger.LogInformation("====================> Products retrieved from cache.");
            }

            return View(products);
        }




        public async Task<IActionResult> Create()
        {
            string token = Request.Cookies["JWT"];
            if(token == null)
                return RedirectToAction("SignIn", "Auth");
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            string roleClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (roleClaim == "Customer")
            {
                string error = "You do not have permission to perform this action.";
                return View("Error", new { message = error });
                //TEMPORARILY 
                //SHOULD REDIRECT THE USER TO PAGE WITH MESSAGE NOT ACCESBILE
            }
            if (!_memoryCache.TryGetValue("Categories", out IEnumerable<Category> categories))
            {
                categories = await _categoryRepository.GetAll();
                _memoryCache.Set("Categories", categories, TimeSpan.FromMinutes(10));
            }

                _logger.LogInformation("=========> categories got retrieved ");
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
            //}

            //return RedirectToAction("Index","Home");
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model,
                                        IFormFile posterFile,
                                        string? categoryName,
                                        string? categoryDesc
                                        )
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

                if (!categoryName.IsNullOrEmpty())
                {
                    Category category = new Category();
                    category.Name = categoryName;
                    category.Description = categoryDesc;
                    await _categoryRepository.Create(category);
                    model.IdCategory = category.Id;
                }

                await _productRepository.Create(model);
                _logger.LogInformation("=========> Product created " + model);

                // Update the product list in the cache
                bool exist = _memoryCache.TryGetValue("Products", out IEnumerable<Product> products);
                if (exist)
                {
                    // Add the newly created product to the cached list
                    List<Product> updatedProducts = products.ToList();
                    updatedProducts.Add(model);
                    _memoryCache.Set("Products", updatedProducts, TimeSpan.FromMinutes(10));
                    _logger.LogInformation("Product added to cache.");
                }
                else
                {
                    _logger.LogWarning("Products not found in cache. No update performed.");
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }


        public async Task<IActionResult> Error(string message)
        {
            ViewData["ErrorMessage"] = message;
            return View();
        }


        public async Task<IActionResult> Edit(int? Id)
        {
            var token = Request.Cookies["JWT"];

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            var roleClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (roleClaim == "Customer")
            {
                string error =  "You do not have permission to perform this action.";
                return View("Error", new { message = error });            
                
            }

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
                        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "pics", Path.GetFileName(existingProduct.Poster));
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

                // Update other properties
                existingProduct.Name = model.Name;
                existingProduct.Description = model.Description;
                existingProduct.Price = model.Price;
                existingProduct.IdCategory = model.IdCategory;
                existingProduct.Poster = model.Poster;

                await _productRepository.Update(existingProduct);
                _logger.LogInformation("=========> Product get updated " + existingProduct);

                // Update the product list in the cache
                bool exist = _memoryCache.TryGetValue("Products", out IEnumerable<Product> products);
                if (exist)
                {
                    IEnumerable<Product> updatedProducts = products.Select(p => p.Id == existingProduct.Id ? existingProduct : p);
                    _memoryCache.Set("Products", updatedProducts, TimeSpan.FromMinutes(10));
                    _logger.LogInformation("Product updated in cache.");
                }
                else
                {
                    _logger.LogWarning("Products not found in cache. No update performed.");
                }

                return RedirectToAction("Index"); return RedirectToAction("Index", "Product");
            }

            // Handle invalid model state here
            return View(model);
        }
















        public async Task<IActionResult> SearchByCategory(int categoryId, string searchTerm)
        {
           
            // TEST IF THE CATEGORY IS IN CACHE
            string cacheKey = $"SearchResult_{categoryId}";
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Product> products))
            {
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

                // Cache the products with a specific expiration time
                _memoryCache.Set(cacheKey, products, TimeSpan.FromMinutes(10)); // Cache for 10 minutes
                _logger.LogInformation("====>products with category => " + categoryId + " and name => " + searchTerm + "get retrieve");
            }
            else
            {
                _logger.LogInformation("====>products with category => " + categoryId + " and name => " + searchTerm + "get retrieve from CACHE");
            }
            
            return View("Index", products);
        }













        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? prdId)
        {
            if (prdId == null || prdId == 0)
                return NotFound();

            Product prdFound = await _productRepository.GetById(prdId);
            if (prdFound == null)
                return Json(new { success = false, message = "Error during the suppression" });

            string posterFileName = Path.GetFileName(prdFound.Poster);

            var picsFolder = "pics"; // Your image folder name
            var webRootPath = _webHostEnvironment.WebRootPath;
            var oldPath = Path.Combine(webRootPath, picsFolder, posterFileName);

            if (System.IO.File.Exists(oldPath))
            {
                _logger.LogInformation("------> Removing Poster");
                System.IO.File.Delete(oldPath);
            }
            else
            {
                _logger.LogError("------> Poster NOT FOUND");
            }

            // Remove the product from the cache
            bool exist = _memoryCache.TryGetValue("Products", out IEnumerable<Product> products);
            if (exist)
            {
                // If the product list is in the cache, remove the deleted product
                IEnumerable<Product> updatedProducts = products.Where(p => p.Id != prdId).ToList();

                // Check if the count changed to ensure the product was removed
                if (updatedProducts.Count() != products.Count())
                {
                    _memoryCache.Set("Products", updatedProducts, TimeSpan.FromMinutes(10));
                }
                else
                {
                    // If the count remains the same, the product might not have been found in the cache
                    _logger.LogWarning("Product not found in cache. No update performed.");
                }
            }
            else
            {
                _logger.LogWarning("Products not found in cache. No update performed.");
            }

            await _productRepository.Delete(prdFound);
            return RedirectToAction("Index");
        }




    }
}
