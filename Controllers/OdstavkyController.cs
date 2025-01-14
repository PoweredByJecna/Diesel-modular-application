using System.Security.AccessControl;
using System.Security.Cryptography;
using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Diesel_modular_application.KlasifikaceRule;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Diesel_modular_application.Services;
using System.Diagnostics;
using AspNetCoreGeneratedDocument;
using Humanizer;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace Diesel_modular_application.Controllers
{
    public class OdstavkyController : Controller
    {
        private readonly DAdatabase _context;
        private readonly DieslovaniController _dieslovani;
        private readonly OdstavkyService _odstavkyService;

        public OdstavkyController(DAdatabase context, OdstavkyService odstavkyService, DieslovaniController dieslovani)
        {
            _context = context;
            _odstavkyService = odstavkyService;
            _dieslovani = dieslovani;
        }
        private List<string> Zpravy { get; set; } = new List<string>();

        [Authorize]
        public async Task<IActionResult> IndexAsync(OdstavkyViewModel odstavky, int page = 1)
        {

   
            int pagesize = 10;
            odstavky.OdstavkyList = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .ToListAsync();
            odstavky.PohotovostList = await _context.Pohotovts
                .Include(o => o.Technik)
                .ToListAsync();
            odstavky.PohotovostList = await _context.Pohotovts
                .Include(O => O.User)
                .ToListAsync();
            odstavky.TechnikList = await _context.TechniS
                .Include(o => o.Firma)
                .ToListAsync();
            odstavky.RegionyList = await _context.ReginoS
                .Include(O => O.Firma)
                .ToListAsync();
            odstavky.DieslovaniList = await _context.DieslovaniS
                .Include(o => o.Technik)
                .ToListAsync();

            var odstavkyQuery = _context.OdstavkyS
                .Include(o => o.Lokality)
                .OrderBy(o => o.IdOdstavky);

            odstavky.OdstavkyList = await odstavkyQuery
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToListAsync();

            odstavky.RegionStats = _odstavkyService.GetRegionStats();


            var id = await _context.DieslovaniS
            .Include(o => o.Technik)
            .Where(static o => o.Technik.Taken == true)
            .Select(o => o.IdTechnik)
            .ToListAsync();
            
            return View("Index", odstavky);
        }
        public async Task<IActionResult> Search(OdstavkyViewModel search, string query, int page = 1)
        {
            int pageSize = 10; // nastavte počet záznamů na stránku

            List<TableOdstavky> FilteredList;
            if (string.IsNullOrEmpty(query))
            {
                FilteredList = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                search.OdstavkyList = FilteredList;
            }
            else
            {
                FilteredList = await _context.OdstavkyS
                    .Include(o => o.Lokality)
                    .Where(o => o.Lokality.Lokalita.Contains(query))
                    .Take(pageSize)
                    .ToListAsync();
                search.OdstavkyList = FilteredList;
            }
            return PartialView("_LokalityListPartial", search);
        }
        
        public async Task<TableDieslovani> Create(OdstavkyViewModel odstavky)
        {
            var lokalitaSearch = await _context.LokalityS
            .Include(l => l.Region)
            .ThenInclude(r => r.Firma)
            .FirstOrDefaultAsync(input => input.Lokalita == odstavky.AddOdstavka.Lokality.Lokalita);





            // var dieslovani = await _dieslovani.HandleOdstavkyDieslovani(lokalitaSearch, odstavky.AddOdstavka.Od, odstavky.AddOdstavka.Do, odstavky, odstavky.AddOdstavka.Popis, newOdstavka, result);
            return null; //upravit
        }
        private async Task<TableLokality?> GetLokalitaAsync(OdstavkyViewModel odstavky)
        {
            return await _context.LokalityS
                .Include(l => l.Region)
                .ThenInclude(r => r.Firma)
                .FirstOrDefaultAsync(input => input.Lokalita == odstavky.AddOdstavka.Lokality.Lokalita);

        }

        

        private string DetermineDistributor(string NazevRegionu)
        {
            return NazevRegionu switch
            {
                "Severní Čechy" or "Západní Čechy" or "Severní Morava" => "ČEZ",
                "Jižní Morava" or "Jižní Čechy" => "EGD",
                "Praha + Střední Čechy" => "PRE",
                _ => ""
            };
        }

        

        private TableOdstavky CreateNewOdstavka(OdstavkyViewModel odstavky, TableLokality lokalitaSearch, string distrib, DateTime od, DateTime do_, string popis)
        {
            Debug.WriteLine($"vytváří se odstávka s parametry: {distrib }, {lokalitaSearch.Region.Firma.NázevFirmy}, {od}, {do_}, {popis}, {lokalitaSearch.Id} ");
            return new TableOdstavky
            {
                Distributor = distrib,
                Firma = lokalitaSearch.Region.Firma.NázevFirmy,
                Od = od,
                Do = do_,
                Popis = popis,
                LokalitaId = lokalitaSearch.Id   
            };
            
        }
        public class HandleOdstavkyDieslovaniResult
        {
            public bool Success { get; set; } // Určuje, zda metoda uspěla
            public string Message { get; set; } // Detailní zpráva o výsledku
            public TableDieslovani? Dieslovani { get; set; } // Vytvořený objekt Dieslovani (pokud existuje)
            public TableOdstavky? Odstavka { get; set; } // Vytvořená odstávka (pokud existuje)
        }


        public async Task<IActionResult> Test(OdstavkyViewModel odstavky)
        {
            var result = new HandleOdstavkyDieslovaniResult();

            try
            {

                var number = await _context.LokalityS.CountAsync();
                if (number == 0)
                {
                    return Json(new { success = false, message = "Chyba při zakladání." });
                }

                var IdNumber = RandomNumberGenerator.GetInt32(1, number);
                var lokalitaSearch = await _context.LokalityS
                    .Include(o => o.Region)
                    .ThenInclude(p => p.Firma)
                    .FirstOrDefaultAsync(i => i.Id == IdNumber);


                if (lokalitaSearch == null)
                {
                    return Json(new { success = false, message = "Lokalita nenalezena." });
                }
                
                Debug.WriteLine($"Vybraná lokalita: {lokalitaSearch.Lokalita}, Region: {lokalitaSearch.Region.NazevRegionu}");

                var hours = RandomNumberGenerator.GetInt32(1, 100);
                string popis = "test";

                string distrib = DetermineDistributor(lokalitaSearch.Region.NazevRegionu);
                Debug.WriteLine($"Distributor: {distrib}");

                var od = DateTime.Today.AddHours(hours + 2);
                var do_ = DateTime.Today.AddHours(hours + 8);

                result = await OdstavkyCheck(lokalitaSearch,od,do_, result);

                if (!result.Success)
                {
                    return Json(new { success = false, message = result.Message });
                }
                
                var newOdstavka = CreateNewOdstavka(odstavky, lokalitaSearch, distrib, od, do_, popis);
                try
                {
                    _context.Add(newOdstavka);
                    await _context.SaveChangesAsync();
                    result.Odstavka = newOdstavka;
                    result.Message = "Odstávka byla úspěšně vytvořena.";
                    Debug.WriteLine($"Id odstavky: {newOdstavka.IdOdstavky}");
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = "Chyba při ukládání do databáze";
                    return Json(new { success = false, message = result.Message });

                }



                result = await _dieslovani.HandleOdstavkyDieslovani(lokalitaSearch, DateTime.Today.AddHours(hours + 2), DateTime.Today.AddHours(hours + 8), odstavky, popis, newOdstavka, result);

                if (!result.Success)
                {
                    return Json(new { success = false, message = result.Message });
                }
                
                return Json(new
                {
                    success = true,
                    message = result.Message,
                    odstavkaId = result.Odstavka?.IdOdstavky,
                    dieslovaniId = result.Dieslovani?.IdDieslovani
                });                          
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Neočekávaná chyba: {ex.Message}" });
            }
        }

        


        private bool ISvalidDateRange(DateTime od, DateTime Do)
        {
            return od.Date >= DateTime.Today && od < Do;
        }
      

        private bool ExistingOdstavka(int lokalitaSearchId, DateTime od)
        {
            var existingOdstavka = _context.OdstavkyS
            .FirstOrDefault(o => o.Od.Date == od.Date && o.Lokality.Id == lokalitaSearchId);
            
            if (existingOdstavka == null)
            {
                return true;
            }
            else 
            { 
                Debug.WriteLine($"Odstávka je již založena: {existingOdstavka.IdOdstavky} na tento den: {existingOdstavka.Od.Date}");
                return false;
            }
        }


        public async Task<IActionResult> Delete(int idodstavky)
        {
            try
            {
                var odstavka = await _context.OdstavkyS.FindAsync(idodstavky);
                if (odstavka == null)
                {
                    return Json(new { success = false, message = "Záznam nebyl nalezen." });
                }

                var dieslovani = await _context.DieslovaniS.Where(p => p.IDodstavky == odstavka.IdOdstavky).FirstOrDefaultAsync();
                if (dieslovani != null)
                {
                    var technik = await _context.TechniS.Where(p => p.IdTechnika == dieslovani.IdTechnik).FirstOrDefaultAsync();
                    if (technik != null)
                    {
                        technik.Taken = false;
                        _context.TechniS.Update(technik);
                    }
                }

                _context.OdstavkyS.Remove(odstavka);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Záznam byl úspěšně smazán." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Chyba při mazání záznamu: " + ex.Message });
            }
        }
        private async Task<HandleOdstavkyDieslovaniResult> OdstavkyCheck(TableLokality lokalitaSearch, DateTime od, DateTime do_, HandleOdstavkyDieslovaniResult result)
        {
            if (!ExistingOdstavka(lokalitaSearch.Id, od))
            {
                Debug.WriteLine($"Již existuje odstávka na " + od);
                result.Success = false;
                result.Message = "Již existuje jiná odstávka.";
                return result;

            }

            if (!ISvalidDateRange(od, do_))
            {
                Debug.WriteLine($"Špatně zadané datum");
                result.Success = false;
                result.Message = "Špatně zadané datum.";
                return result;
            }
            else
            {
                return result;
            }
        } 
         public async Task<IActionResult> GetTableData(int start = 0, int length = 0)
        {
            // Celkový počet záznamů v tabulce
            int totalRecords = await _context.OdstavkyS.CountAsync();
            length = totalRecords;
            
            // Načtení záznamů pro aktuální stránku
            var odstavkyList = await _context.OdstavkyS
                .Include(o=>o.Lokality)
                .OrderBy(o => o.Od) // Nebo jiný řadící sloupec
                .Skip(start)
                .Take(length)   
                .Select(l => new
                {
                    l.IdOdstavky,
                    l.Distributor,
                    l.ZadanOdchod,
                    l.ZadanVstup,
                    l.Lokality.Lokalita,
                    l.Lokality.Klasifikace,
                    l.Od,
                    l.Do,
                    l.Lokality.Adresa,
                    l.Lokality.Baterie,
                    l.Popis,
                    l.Lokality.Zasuvka,

                    idTechnika = _context.DieslovaniS
                    .Where(d => d.IDodstavky == l.IdOdstavky)
                    .Select(d => d.Technik.IdTechnika)
                    .FirstOrDefault()


                })
                .ToListAsync();
            

            // Vrácení dat ve formátu očekávaném DataTables
            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(), // Unikátní ID požadavku
                recordsTotal = totalRecords, // Celkový počet záznamů
                recordsFiltered = totalRecords, // Může být upraven při vyhledávání
                data = odstavkyList // Data aktuální stránky
            });
        }




    }

}