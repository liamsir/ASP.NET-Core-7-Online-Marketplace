using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MVCWebAppIsmane.Models;
using MVCWebAppIsmane.Repositories.IRepositories;
using Newtonsoft.Json;

namespace MVCWebAppIsmane.Controllers
{
    public class PanierController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;


        public PanierController(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            
            _categoryRepository = categoryRepository;
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

            /*// Transform the retrieved products into ProductCartItem instances with quantity
            var productsWithQuantity = productsInCart.Select(product =>
            {
                var quantity = cartItemIds.Count(id => id == product.Id);
                return new ProductPanierMV { Product = product, Quantity = quantity };
            }).ToList();*/ // Convert the result to a List<ProductCartItem>

            return View(productsInCart);
        }
















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














        public async Task<IActionResult> AddToCart(int productId)
        {
            // Retrieve the cart items from cookies
            List<Product> cartItems = new List<Product>();
            
            var existingCart = Request.Cookies["CartItems"];

            if (!string.IsNullOrEmpty(existingCart))
            {
                try
                {
                    cartItems = JsonConvert.DeserializeObject<List<Product>>(existingCart);
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine("Error deserializing JSON: " + ex.Message);
                }
            }

            // Check if the product is already in the cart, update its quantity
            Product existingProduct = cartItems.FirstOrDefault(item => item.Id == productId);
            if (existingProduct != null)
            {
                Console.WriteLine("Product already in cart");
            }
            else
            {
                // Retrieve the product information based on productId (assuming _productRepository.GetById(productId) fetches the product)
                Product product = await _productRepository.GetById(productId);

                if (product != null)
                {
                    // If the product is not in the cart, add it with a quantity of 1
                    cartItems.Add(product);
                }
            }

            // Save the updated cart items in cookies
            string cartJson = JsonConvert.SerializeObject(cartItems);
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7)
            };
            Response.Cookies.Append("CartItems", cartJson, option);

            return View();
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

            return RedirectToAction("Index");
        }



        








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