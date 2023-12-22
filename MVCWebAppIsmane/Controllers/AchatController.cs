using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MVCWebAppIsmane.Models;
using MVCWebAppIsmane.Repositories;
using MVCWebAppIsmane.Repositories.IRepositories;
using System.Security.Claims;

namespace MVCWebAppIsmane.Controllers
{
    public class AchatController : Controller
    {

        private readonly IAchatRepository _achatRepository;
        private readonly ILigneAchatRepository _ligneAchatRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<AchatController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly UserManager<User> _userManager;



        public AchatController( IAchatRepository achatRepository,
                                ILigneAchatRepository ligneAchatRepository,
                                IProductRepository productRepository,
                                ILogger<AchatController> logger,
                                IMemoryCache memoryCache,
                                UserManager<User> userManager
                               )
        {

            _achatRepository = achatRepository;
            _ligneAchatRepository = ligneAchatRepository;
            _productRepository = productRepository;
            _logger = logger;
            _memoryCache = memoryCache;
            _userManager = userManager;

        }





        
        public async Task<IActionResult> ViewProducts(List<int> ids)
        {
            try
            {
                var products = new List<Product>();

                foreach (var id in ids)
                {
                    string cacheKey = $"Product_{id}";

                    if (!_memoryCache.TryGetValue(cacheKey, out Product product))
                    {
                        product = await _productRepository.GetById(id);
                        if (product != null)
                        {
                            _memoryCache.Set(cacheKey, product, TimeSpan.FromMinutes(10)); // Cache for 10 minutes
                        }
                    }

                    if (product != null)
                    {
                        products.Add(product);
                        _logger.LogInformation("Product to buy is  " + product.Name);
                    }
                }
                _logger.LogInformation("=======> Products size to buy is " + products.Count());
                return View("Index", products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while viewing products."); // Log an error if an exception occurs
                throw; // Rethrow the exception to maintain expected behavior
            }
        }


        
        [HttpPost]
        public async Task<IActionResult> BuyProducts(List<int> productIds, List<int> quantities)
        {
            string token = Request.Cookies["JWT"];
            
            if (!string.IsNullOrEmpty(token))
            {

                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
                string roleClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleClaim != "Customer")
                    return RedirectToAction("Privacy", "Home");
                string userEmailClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
                if (!string.IsNullOrEmpty(userEmailClaim))
                {
                    


                    try
                    {
                        if (productIds != null && quantities != null && productIds.Count == quantities.Count)
                        {
                            // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            Achat achat = new Achat();
                            var user = await _userManager.FindByEmailAsync(userEmailClaim);
                            if (user == null)
                            {
                                return RedirectToAction("SignUp", "Auth");
                            }
                            _logger.LogInformation($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>> {userEmailClaim}===> {user.Id}");
                            achat.IdUser = user.Id;
                            await _achatRepository.Create(achat); // Save the new Achat first to generate its Id

                            for (int i = 0; i < productIds.Count; i++)
                            {
                                int productId = productIds[i];
                                int quantite = quantities[i];

                                // Check if the product is already cached
                                string cacheKey = $"Product_{productId}";
                                if (!_memoryCache.TryGetValue(cacheKey, out Product product))
                                {
                                    product = await _productRepository.GetById(productId);
                                    if (product != null)
                                    {
                                        _memoryCache.Set(cacheKey, product, TimeSpan.FromMinutes(10)); // Cache for 10 minutes
                                    }
                                }

                                if (product != null)
                                {
                                    if(quantite <= product.QuantityStock)
                                    {
                                        product.QuantityStock = product.QuantityStock - quantite;
                                        await _productRepository.Update(product);

                                        LigneAchat ligneAchat = new LigneAchat();
                                        ligneAchat.IdAchat = achat.Id;
                                        ligneAchat.IdProduct = productId;
                                        ligneAchat.quantity = quantite;
                                        
                                        await _ligneAchatRepository.Create(ligneAchat);
                                        achat.LigneAchats.Add(ligneAchat);
                                        _logger.LogInformation("====>ligne Achat with product " + ligneAchat.IdProduct + " get created");
                                    }
                                    else
                                    {
                                        string errorMessage = $"Sorry, we only have {product.QuantityStock} item{(product.QuantityStock == 1 ? "" : "s")} available for this product. You're attempting to buy {quantite}.";
                                        return RedirectToAction("Error", "Product", new { message = errorMessage });
                                    }
                                }
                            }

                            await _achatRepository.Update(achat); // Update Achat with LigneAchats references
                            _logger.LogInformation("====> CLIENT WITH ID => " + achat.IdUser + " effectue un achat");
                            _memoryCache.Remove("Products");
                            return RedirectToAction("Index", "Product");
                        }

                        //return View("Index", "Home");

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, " ------------------------------------------------------------------------------------------------------------------------------>Error occurred while buying products."); // Log an error if an exception occurs
                        throw; // Rethrow the exception to maintain expected behavior
                    }
                }
            }
            return RedirectToAction("SignIn", "Auth");
            
        }


    }
}
