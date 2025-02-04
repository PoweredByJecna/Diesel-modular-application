using Diesel_modular_application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Diesel_modular_application.Controllers
{
    public class UserController(UserService service) : Controller
    {
        private readonly UserService _service = service;

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DetailUserJson(string id)
        {
            var detailUser = await _service.DetailUserJsonAsync(id);
            return Json(new
            {
                data=detailUser
            });
        }

    }
}