using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;


namespace Diesel_modular_application.Controllers
{ 
public class LogOutController(SignInManager<IdentityUser> signInManager, ILogger<LogOutController> logger) : Controller
{
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly ILogger<LogOutController> _logger = logger;

        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return Redirect("/Login/Index");
            }
        }
}
}