
using Microsoft.AspNetCore.Mvc;
using MVCWebAppIsmane.Models;
using MVCWebAppIsmane.Repositories.IRepositories;
using MVCWebAppIsmane.Security.service;


namespace MVCWebAppIsmane.Controllers
{
    public class LoginVMController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IJwtService _jwtServiceProvider;
        private readonly ILogger<LoginVM> _logger;



        public LoginVMController(IClientRepository clienticlientRepository, IJwtService jwt, ILogger<LoginVM> logger)
        {
            _clientRepository = clienticlientRepository;
            _jwtServiceProvider = jwt;
            _logger = logger;
        }

        public IActionResult SignIn()
        {

            return View("SignIn");
        }

        public IActionResult SignUp()
        {
            return View("SignUp");
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            Client client = await userExist(model.Email, model.Password);

            if (client != null)
            {
                string token = _jwtServiceProvider.GenerateJwtToken(client.Email);

                // Set JWT in an HTTP-only cookie
                Response.Cookies.Append("JWT", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Enable this for HTTPS only (recommended)
                                   // Other cookie settings can be applied as needed (expiration, domain, etc.)
                });

                return RedirectToAction("Index", "Product"); 
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View("SignIn", model);
        }




        [HttpPost]
        public async Task<IActionResult> Register(Client model)
        {
            if (ModelState.IsValid)
            {
                // Hash the password before storing it
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Replace the plain password with the hashed one
                model.Password = hashedPassword;


                // Code to save the model to your database goes here
                await _clientRepository.Create(model);


                // Redirect to a different view after successful registration
                return View("SignIn");
            }

            // If the model state is invalid, return to the registration view
            return View("SignUp", model);
        }


/*
 
        private async Task<Client> userExist(string username, string password)
        {
            IEnumerable<Client> clients = await _clientRepository.Get(c => c.Email == username && c.Password == password);

            if (clients != null && clients.Any())
            {
                return clients.First(); // Return the first found client
            }

            return null; // Return null if no client is found
        }


*/

        private async Task<Client> userExist(string email, string password)
        {
            IEnumerable<Client> clients = await _clientRepository.Get(c => c.Email == email);
            Client client = null;
            if (clients != null && clients.Any())
            {
                client =  clients.First(); 
            }
            
            bool passwordMatches = BCrypt.Net.BCrypt.Verify(password, client.Password);
            
            return passwordMatches ? client : null;
            
        }

    }
}
