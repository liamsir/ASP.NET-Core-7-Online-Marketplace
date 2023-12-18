
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCWebAppIsmane.Models;
using MVCWebAppIsmane.Repositories.IRepositories;
using MVCWebAppIsmane.Security.service;


namespace MVCWebAppIsmane.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _UserRepository;
        private readonly IJwtService _jwtServiceProvider;
        private readonly ILogger<LoginVM> _logger;


        public AuthController(UserManager<User> userManager, IUserRepository UseriUserRepository, IJwtService jwt, ILogger<LoginVM> logger)
        {
            _userManager = userManager;
            _UserRepository = UseriUserRepository;
            _jwtServiceProvider = jwt;
            _logger = logger;
        }

        public IActionResult SignUp()
        {
            return View("SignUp");
        }

        public IActionResult SignIn()
        {

            return View("SignIn");
        }
        

        public IActionResult SignOut()
        {
            Response.Cookies.Delete("JWT");
            
            _logger.LogInformation($"====>User logged out at {DateTime.UtcNow}");
            return RedirectToAction("Index","Product");
        }

        



        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {            
            User user = await userExist(model.Email, model.Password);
            DateTime loginTime = DateTime.UtcNow;

            if (user != null)
            {
                bool isCustomer = await _userManager.IsInRoleAsync(user, "Customer");
                string token = _jwtServiceProvider.GenerateJwtToken(user.Email, isCustomer ? "Customer" : "Admin");

                // Set JWT in an HTTP-only cookie
                Response.Cookies.Append("JWT", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Ena   ble this for HTTPS only (recommended)
                                   // Other cookie settings can be applied as needed (expiration, domain, etc.)
                });
                _logger.LogInformation($"====>User with {user.Email} logged in at {loginTime}");
                return RedirectToAction("Index", "Product");
            
            }           
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
           _logger.LogWarning($"Failed login attempt for user {model.Email} at {loginTime}"); // Log failed login attempt           
            return View("SignIn", model);
        }




        [HttpPost]
        public async Task<IActionResult> Register(User model,string UserRole)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);
                model.PasswordHash = hashedPassword;

                model.UserName = model.Email;

                // Create the user using the repository or your preferred method
                await _UserRepository.Create(model);

                // Assign the role to the user
                await _userManager.AddToRoleAsync(model, UserRole); // or "Admin" as needed

                
                _logger.LogInformation($"====>User {model.Email} registered at {DateTime.UtcNow} as {UserRole}");
                // Redirect to a different view after successful registration
                return View("SignIn");
            }

            // If the model state is invalid, return to the registration view
            return View("SignUp", model);
        }




        private async Task<User> userExist(string email, string password)
        {
            IEnumerable<User> Users = await _UserRepository.Get(c => c.Email == email);
            bool passwordMatches = false;
            User User = null;
            if (Users != null && Users.Any())
            {
                User =  Users.First();
                passwordMatches = BCrypt.Net.BCrypt.Verify(password, User.PasswordHash);
            }
            
            
            return passwordMatches ? User : null;
            
        }

    }
}
























        /*

                private async Task<User> userExist(string username, string password)
                {
                    IEnumerable<User> Users = await _UserRepository.Get(c => c.Email == username && c.Password == password);

                    if (Users != null && Users.Any())
                    {
                        return Users.First(); // Return the first found User
                    }

                    return null; // Return null if no User is found
                }


        */