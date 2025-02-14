using Diesel_modular_application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Diesel_modular_application.Controllers
{
    public class LogController(LogService logService) : Controller
    {
        private readonly LogService _logService = logService;

        // ----------------------------------------
        // Poslani logu pro dieslování do ajax
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetLogByEntity(int entityId)
        {   
            var logDieslovani = await _logService.GetLogByEntityAsync(entityId);
            return Json(new
            {
                data=logDieslovani
            });

        }
    }
}