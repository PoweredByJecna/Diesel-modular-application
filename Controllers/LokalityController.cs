
using Diesel_modular_application.Models;
using Diesel_modular_application.Data;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Office2016.Excel;

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
        public async Task<IActionResult> IndexAsync(OdstavkyViewModel lokality)
        {
            lokality.LokalityList = await _context.LokalityS.ToArrayAsync();
            lokality.FirmaList =await _context.FrimaS.ToListAsync();
            lokality.RegionyList = await _context.ReginoS.ToListAsync();

            return View("Index",lokality);
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