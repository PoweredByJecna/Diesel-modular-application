using Diesel_modular_application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Diesel_modular_application.Controllers
{

    public class LoginController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginController> _logger;

        public LoginController(SignInManager<IdentityUser> signInManager, ILogger<LoginController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Input.UserName, model.Input.Password, model.Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return Redirect("/Dieslovani/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Redirect("/Dieslovani/Index");
                }
            }
            return View("Index", model);
        }
        public async Task<IActionResult> Logout(Login model)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
             return Redirect("/Dieslovani/Index");
        }
    }
}
