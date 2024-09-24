
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Diesel_modular_application.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Diesel_modular_application.Controllers;


namespace Diesel_modular_application.Controllers
{ 
public class LogOutController:Controller
{
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogOutController> _logger;

        public LogOutController(SignInManager<IdentityUser> signInManager, ILogger<LogOutController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

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