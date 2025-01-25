using System.Diagnostics;
using System.Threading.Tasks;
using Diesel_modular_application.Data;
using Diesel_modular_application.KlasifikaceRule;
using Diesel_modular_application.Models;
using Diesel_modular_application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diesel_modular_application.Controllers
{
    public class DieslovaniController : Controller
    {
        private readonly DieslovaniService _dieslovaniService;
        private readonly UserManager<IdentityUser> _userManager;

        public DieslovaniController(
            DieslovaniService dieslovaniService,
            UserManager<IdentityUser> userManager)
        {
            _dieslovaniService = dieslovaniService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer"))
            {
                TempData["TableName"] = "Moje";
            }
            else
            {
                TempData["TableName"] = "";
            }
            return View();
        }

        // -------------------------------
        // Zobrazení detailu Dieslovani
        // -------------------------------
        [HttpGet]
        public async Task<IActionResult> DetailDieslovani(int id)
        {
            var detail = await _dieslovaniService.DetailDieslovaniAsync(id);
            if (detail == null)
                return NotFound();

          

            return View(detail);
        }

        // ----------------------------------------
        // Vstup - volá metodu ze servisu
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> Vstup(int IdDieslovani)
        {
            var (Success, Message) = await _dieslovaniService.VstupAsync(IdDieslovani);

            if (!Success)
            {
                return Json(new { success = false, message = Message });
            }
            else
            {
                return Json(new
                {
                    success = true,
                    message = Message,
                    tempMessage = TempData["Zprava"] // např. pro modal
                });
            }
        }

        // ----------------------------------------
        // Odchod - volá metodu ze servisu
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> Odchod(int IdDieslovani)
        {
            var (Success, Message) = await _dieslovaniService.OdchodAsync(IdDieslovani);

            if (!Success)
            {
                return Json(new { success = false, message = Message });
            }
            else
            {
                return Json(new { success = true, message = Message });
            }
        }

        // ----------------------------------------
        // Dočasný odchod/obnovení (TemporaryLeave)
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> TemporaryLeave(int IdDieslovani)
        {
            var (Success, Message) = await _dieslovaniService.TemporaryLeaveAsync(IdDieslovani);
            if (!Success)
            {
                return Json(new { success = false, message = Message });
            }
            else
            {
                // Příklad: redirect nebo prostě JSON
                return Redirect("/Dieslovani/Index");
                // nebo:
                // return Json(new { success = true, message = Message });
            }
        }

        // ----------------------------------------
        // Převzetí dieslování (Take)
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> Take(int IdDieslovani)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var (Success, Message, TempMessage) = await _dieslovaniService.TakeAsync(IdDieslovani, currentUser);

            if (!Success)
            {
                return Json(new { success = false, message = Message });
            }
            else
            {
                // Předáme do TempData, pokud chceme zobrazit ve view
                if (TempMessage != null) TempData["Zprava"] = TempMessage;

                return Json(new
                {
                    success = true,
                    message = Message,
                    tempMessage = TempData["Zprava"]
                });
            }
        }

        // ----------------------------------------
        // Tabulky (původně GetTableDataRunningTable atd.)
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetTableDataRunningTable(int start = 0, int length = 0)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isEngineer = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer");

            var (totalRecords, data) = await _dieslovaniService.GetTableDataRunningTableAsync(currentUser, isEngineer);

            // Vrátíme data ve formátu DataTables
            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetTableDataAllTable(int start = 0, int length = 0)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isEngineer = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer");

            var (totalRecords, data) = await _dieslovaniService.GetTableDataAllTableAsync(currentUser, isEngineer);

            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetTableDatathrashTable(int start = 0, int length = 0)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isEngineer = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer");

            var (totalRecords, data) = await _dieslovaniService.GetTableDatathrashTableAsync(currentUser, isEngineer);

            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetTableUpcomingTable(int start = 0, int length = 0)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isEngineer = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer");

            var (totalRecords, data) = await _dieslovaniService.GetTableUpcomingTableAsync(currentUser, isEngineer);

            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetTableDataEndTable(int start = 0, int length = 0)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isEngineer = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer");

            var (totalRecords, data) = await _dieslovaniService.GetTableDataEndTableAsync(currentUser, isEngineer);

            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }

        // ----------------------------------------
        // Smazání dieslování
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> Delete(int iDdieslovani)
        {
            var (Success, Message) = await _dieslovaniService.DeleteDieslovaniAsync(iDdieslovani);
            if (!Success)
            {
                return Json(new { success = false, message = Message });
            }
            else
            {
                return Json(new { success = true, message = Message });
            }
        }
    }
}
