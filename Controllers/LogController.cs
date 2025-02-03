using Diesel_modular_application.Data;
using Diesel_modular_application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Diesel_modular_application.Controllers
{
    public class LogController:Controller
    {
        private readonly LogService _logService;
        public LogController(LogService logService)
        {
            _logService=logService;
        }

        // ----------------------------------------
        // Poslani logu pro dieslování do ajax
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetLogByEntity(int id)
        {   
            var logDieslovani = await _logService.GetLogByEntityAsync(id);
            return Json(new
            {
                data=logDieslovani
            });

        }
    }
}