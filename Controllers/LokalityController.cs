
using Diesel_modular_application.Models;
using Diesel_modular_application.Data;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.EntityFrameworkCore.Query.Internal;

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
        public async Task<IActionResult> IndexAsync(OdstavkyViewModel lokality, int page=1)
        {
            lokality.LokalityList = await _context.LokalityS.ToListAsync();
            lokality.FirmaList =await _context.FrimaS.ToListAsync();
            lokality.RegionyList = await _context.ReginoS.ToListAsync();
            int pagesize=10;
            var odstavkyQuery = _context.LokalityS
                .OrderBy(o=>o.Id);    

            lokality.LokalityList= await odstavkyQuery
                .Skip((page-1)*pagesize)
                .Take(pagesize)
                .ToListAsync();
            lokality.CurrentPage=page;
            lokality.TotalPages=(int)Math.Ceiling(await odstavkyQuery.CountAsync()/(double)pagesize);    

            return View("Index",lokality);
        }
        public async Task<IActionResult> Search(OdstavkyViewModel search, string query, int page = 1)
        {
            int pageSize = 10; // nastavte počet záznamů na stránku

            List<TableLokality> FilteredList;
            if (string.IsNullOrEmpty(query))
            {
                FilteredList = await _context.LokalityS
                .Include(o=>o.Region)
                .Skip((page - 1) * pageSize) 
                .Take(pageSize)
                .ToListAsync();
                search.LokalityList = FilteredList;
            }
            else
            {
                FilteredList = await _context.LokalityS
                    .Include(o=>o.Region)
                    .Where(o => o.Lokalita.Contains(query) || o.Region.NazevRegionu.Contains(query) || o.Id.ToString().Contains(query))
                    .Take(pageSize)
                    .ToListAsync();
                search.LokalityList = FilteredList;
            }
            return PartialView("_LokalityListPartial", search);
        }
        [HttpPost]
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
            l.Zásuvka,
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

       [Authorize(Roles ="Admin")]
       public async Task<IActionResult> Nacteni(OdstavkyViewModel lok)
       {
            var lokality = _context.LokalityS.ToList();
            var regionyAMesta = new Dictionary<string, List<string>>
            {
                { "Severní Čechy", new List<string> { "Ústí nad Labem", "Teplice", "Liberec", "Most", "Litoměřice", "Česká Lípa" } },
                { "Jižní Morava", new List<string> { "Brno-město", "Znojmo", "Třebíč", "Uherské Hradiště", "Zlín" } },
                { "Praha + Střední Čechy", new List<string> { "Hlavní město Praha", "Mělník", "Beroun" } },
                { "Severní Morava", new List<string> { "Nový Jičín", "Vsetín", "Náchod", "Žďár nad Sázavou" } },
                { "Západní Čechy", new List<string> { "Blansko", "Havlíčkův Brod", "Trutnov", "Ústí nad Orlicí" } },
                { "Jižní Čechy", new List<string> { "České Budějovice", "Český Krumlov", "Třeboň" } }
            };

            foreach(var lokalityReg in lokality)
            {
                foreach(var Region in regionyAMesta)    
                {
                    if(Region.Value.Any(mesto=>lokalityReg.Adresa.Contains(mesto)))
                    {

                        var RegionDb = await _context.ReginoS.FirstOrDefaultAsync(o=>o.NazevRegionu==Region.Key);
                        {
                            if(RegionDb!=null)
                            lokalityReg.RegionID=RegionDb.IdRegion;
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();

         return View();
       }
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if(file != null && file.Length>0)
            {
                ExcelHandling excelFileHandling = new ExcelHandling();
                var lokalityS = excelFileHandling.ParseExcelFile(file.OpenReadStream());

                await _context.LokalityS.AddRangeAsync(lokalityS);
                await _context.SaveChangesAsync();

                return View("Index");


            }
            return View("Index");
        }
    

    }
}