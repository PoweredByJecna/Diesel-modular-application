using Microsoft.AspNetCore.Mvc;

namespace Diesel_modular_application.Controllers
{
    public class UserController:Controller
    {
          public IActionResult Index()
        {
            return View();
        }

    }
}