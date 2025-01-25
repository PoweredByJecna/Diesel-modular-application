
using Diesel_modular_application.Models;
using Diesel_modular_application.Data;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.EntityFrameworkCore.Query.Internal;
using PagedList;
using System.Collections.ObjectModel;

namespace Diesel_modular_application.Controllers
{
    public class LokalityController : Controller
    {
        private readonly DAdatabase _context;

        public LokalityController(DAdatabase context)
        {
            _context= context;
        }

        [Authorize]
        public  IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetTableData(int start = 0, int length = 0)
        {
            // Celkový počet záznamů v tabulce
            int totalRecords = await _context.LokalityS.CountAsync();
            length = totalRecords;
            // Načtení záznamů pro aktuální stránku
            var LokalityList = await _context.LokalityS
                .Include(o=>o.Region)
                .OrderBy(o => o.Id) // Nebo jiný řadící sloupec
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

            // Vrácení dat ve formátu očekávaném DataTables
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