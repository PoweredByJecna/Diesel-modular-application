using Diesel_modular_application.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Diesel_modular_application.Controllers
{
    public class LokalityController(DAdatabase context) : Controller
    {
        private readonly DAdatabase _context = context;

        [Authorize]
        public  IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetTableData(int start = 0, int length = 0)
        {
             
                int totalRecords = await _context.LokalityS.CountAsync();
                length = totalRecords;
            
                var LokalityList = await _context.LokalityS
                .Include(o=>o.Region)
                .OrderBy(o => o.Id) 
                .Skip(start)
                .Take(length)
                .Select(l => new
                {
                    l.Id,
                    l.Lokalita,
                    l.Klasifikace,
                    l.Adresa,
                    l.Region.NazevRegionu,
                    l.Baterie,
                    l.Zasuvka,
                    l.DA
                })
                .ToListAsync();

       
            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(), // Unikátní ID požadavku
                recordsTotal = totalRecords, // Celkový počet záznamů
                recordsFiltered = totalRecords, // Může být upraven při vyhledávání
                data = LokalityList // Data aktuální stránky
            });
        }

       
    

    }
}