using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MVCWebAppIsmane.Repositories.IRepositories;
using Newtonsoft.Json;

namespace MVCWebAppIsmane.Controllers
{
    public class PanierController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PanierController>  _logger;
        private readonly IProductRepository _productRepository;
        


        public PanierController(IMemoryCache memoryCache, ILogger<PanierController> logger, IProductRepository productRepository)
        {
            _memoryCache = memoryCache;
            _logger = logger;            
            _productRepository = productRepository;
            
        }



        public async Task<IActionResult> Index()
        {

            var existingCart = Request.Cookies["CartItems"];
            List<int> cartItemIds = new List<int>();

            if (!string.IsNullOrEmpty(existingCart))
            {
                try
                {
                    cartItemIds = JsonConvert.DeserializeObject<List<int>>(existingCart);
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine("Error deserializing JSON: " + ex.Message);
                }
            }

            // Retrieve products from your repository using the product IDs
            var productsInCart = await _productRepository.Get(p => cartItemIds.Contains(p.Id));
            /*foreach (var item in productsInCart)
            {
                _logger.LogInformation("====>Product " + item.Name   + "=> "+ item.Price);
            }*/
            /*// Transform the retrieved products into ProductCartItem instances with quantity
            var productsWithQuantity = productsInCart.Select(product =>
            {
                var quantity = cartItemIds.Count(id => id == product.Id);
                return new ProductPanierMV { Product = product, Quantity = quantity };
            }).ToList();*/ // Convert the result to a List<ProductCartItem>
            _logger.LogInformation("====>the CART has "+ productsInCart.Count()+" products");
            return View(productsInCart);
        }




       


        public IActionResult RemoveFromCart(int productId)
        {
            // Retrieve the cart items (IDs) from cookies
            List<int> cartItemIds = new List<int>();
            var existingCart = Request.Cookies["CartItems"];

            if (!string.IsNullOrEmpty(existingCart))
            {
                try
                {
                    cartItemIds = JsonConvert.DeserializeObject<List<int>>(existingCart);
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine("Error deserializing JSON: " + ex.Message);
                }
            }

            // Remove the selected product ID from the cart
            cartItemIds.Remove(productId);

            // Save the updated cart items (IDs) in cookies
            string cartJson = JsonConvert.SerializeObject(cartItemIds);
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7)
            };
            Response.Cookies.Append("CartItems", cartJson, option);
            _logger.LogInformation("product with id "+productId+" get removed from cart");
            return RedirectToAction("Index");
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
                    //Console.WriteLine("Error deserializing JSON: " + ex.Message);
                    _logger.LogError("Error deserializing JSON: " + ex.ToString());
                }

            }

            
            try
            {
                cartItems.Add(productId);

            }
            catch (JsonReaderException ex)
            {
                _logger.LogError("Error ADDING " + productId + " to CART :" + ex.ToString());
            }

            // Save the updated cart items in cookies
            string cartJson = JsonConvert.SerializeObject(cartItems);
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7) // Adjust the expiration time as needed
            };
            Response.Cookies.Append("CartItems", cartJson, option);
            _logger.LogInformation("=================> " + productId + "get added to Cart");
            // Redirect or return a response as needed
            return RedirectToAction("Index", "Product");
        }



       /* public async Task<IActionResult> ViewCart()
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
                    Console.WriteLine("Error deserializing JSON: " + ex.Message);
                }
            }

            if (cartItems.Any())
            {
                if (!_memoryCache.TryGetValue("CartItems", out List<int> cachedCartItems))
                {
                    _memoryCache.Set("CartItems", cartItems, TimeSpan.FromMinutes(10)); // Cache for 10 minutes
                }
            }

            // Fetch products using the cartItems if needed
            var productInCart = await _productRepository.Get(p => cartItems.Contains(p.Id));
            return View("Index", productInCart);
        }*/






    }
}

        /*[HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            // Retrieve the cart items from cookies
            List<ProductPanierMV> cartItems = new List<ProductPanierMV>();
            var existingCart = Request.Cookies["CartItems"];

            if (!string.IsNullOrEmpty(existingCart))
            {
                try
                {
                    
                    cartItems = JsonConvert.DeserializeObject<List<ProductPanierMV>>(existingCart);
                }
                catch (JsonReaderException ex)
                {
                    // Handle the exception or log it
                    Console.WriteLine("Error deserializing JSON: " + ex.Message);
                }
            }

            // Find the item in the cart
            var productToUpdate = cartItems.FirstOrDefault(item => item.Product.Id == productId);
            if (productToUpdate != null)
            {
                // Update the quantity
                productToUpdate.Quantity = quantity;

                // Save the updated cart items in cookies
                string cartJson = JsonConvert.SerializeObject(cartItems);
                CookieOptions option = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7)
                };
                Response.Cookies.Append("CartItems", cartJson, option);
            }

            return RedirectToAction("Index");
        }
*/

        /*public async Task<IActionResult> ViewCart()
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
        }*/