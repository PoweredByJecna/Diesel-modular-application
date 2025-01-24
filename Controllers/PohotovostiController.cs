using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using Diesel_modular_application.Services;

namespace Diesel_modular_application.Controllers
{
    public class PohotovostiController : Controller
    {
        private readonly PohotovostiService _pohotovostiService;
        private readonly UserManager<IdentityUser> _userManager;

        public PohotovostiController(
            PohotovostiService pohotovostiService,
            UserManager<IdentityUser> userManager)
        {
            _pohotovostiService = pohotovostiService;
            _userManager = userManager;
        }

        // ------------------------
        // 1) IndexAsync
        // ------------------------
        public async Task<IActionResult> IndexAsync(OdstavkyViewModel pohotovosti)
        {
            // Načteme pohotovosti ze servisu
            pohotovosti.PohotovostList = await _pohotovostiService.GetAllPohotovostiAsync();

            return View("Index", pohotovosti);
        }

        // ------------------------
        // 2) Zapis
        // ------------------------
        [Authorize(Roles = "Engineer,Admin")]
        public async Task<IActionResult> Zapis(OdstavkyViewModel pohotovosti)
        {
            // Zjistíme aktuálního uživatele
            var currentUser = await _userManager.GetUserAsync(User);

            var (Success, Message) = 
                await _pohotovostiService.ZapisPohotovostAsync(pohotovosti, currentUser);

            if (!Success)
            {
                // Můžete například přidat TempData["Error"] = Message; 
                // a přesměrovat, nebo vrátit JSON... 
                // Zde ukázka přesměrování s ViewBag/TempData
                TempData["Error"] = Message;
                return Redirect("/Odstavky/Index");
            }

            // Po úspěšném zapsání se vrátíme na /Odstavky/Index
            TempData["Message"] = Message;
            return Redirect("/Odstavky/Index");
        }

        // ------------------------
        // 3) GetTableDatapohotovostiTable
        // ------------------------
        public async Task<IActionResult> GetTableDatapohotovostiTable(int start = 0, int length = 0)
        {
            var (totalRecords, data) = 
                await _pohotovostiService.GetPohotovostTableDataAsync(start, length);

            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }
    }
}
