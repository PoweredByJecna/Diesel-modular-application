using Diesel_modular_application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Diesel_modular_application.Controllers
{

    public class LoginController(SignInManager<IdentityUser> signInManager, ILogger<LoginController> logger) : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly ILogger<LoginController> _logger = logger;

        [HttpGet]
        public IActionResult Index(Login login)
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
                    ModelState.AddModelError(string.Empty, "Špatné heslo nebo uživatelské jméno");
                    return View("Index", model);
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
