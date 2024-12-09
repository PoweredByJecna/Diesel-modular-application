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

namespace Diesel_modular_application.Controllers
{
    public class OdstavkyController : Controller
    {
        private readonly DAdatabase _context;
        private readonly OdstavkyService _odstavkyService;
        public OdstavkyController(DAdatabase context, OdstavkyService odstavkyService)
        {
            _context = context;
            _odstavkyService = odstavkyService;
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

            #pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            odstavky.TechnikLokalitaMap = await _context.DieslovaniS
            .Where(d => d.Technik.Taken) // Pouze technici, kteří jsou právě na dieslování
            .GroupBy(d => d.IdTechnik)  // Seskupení podle IdTechnika
            .ToDictionaryAsync(
                group => group.Key,
                group => group.Select(d => d.Odstavka.Lokality.Lokalita).FirstOrDefault()
            );
            #pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
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
        public async Task<IActionResult> HandleOdstavkyDieslovani(TableLokality lokalitaSearch, DateTime od, DateTime do_, OdstavkyViewModel odstavky, string popis)
        {
            try
            {
                if (lokalitaSearch == null)
                {
                    ViewBag.Message = "Zadaná lokalita neexistuje";
                    return View("Index", odstavky);
                }

                string distrib = DetermineDistributor(lokalitaSearch.Region.NazevRegionu);
                var newOdstavka = CreateNewOdstavka(odstavky, lokalitaSearch, distrib, od, do_, popis);

                if (ExistingOdstavka(newOdstavka.LokalitaId, newOdstavka.Do))
               

                if (!ISvalidDateRange(newOdstavka.Od, newOdstavka.Do))
                {
                    TempData["Zprava"] = "Špatně zadané datum";
                    return Redirect("/Home/Odstavky");
                }

                await _context.OdstavkyS.AddAsync(newOdstavka);
                await _context.SaveChangesAsync();

                var technikSearch = await AssignTechnikAsync(newOdstavka, lokalitaSearch, odstavky);
                if (technikSearch == null)
                {
                    Zpravy.Add("Technik nebyl nalezen.");
                    TempData["Zprava"] = string.Join("; ", Zpravy);
                    return Redirect("/Home/Index");
                }

                TempData["Zprava"] = string.Join("; ", $"Technik: {technikSearch.Jmeno} je objednán na dieslovaní", Zpravy);
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Došlo k chybě při zpracování dieslování: " + ex.Message;
                return Redirect("/Home/Index");
            }
        }
        public async Task<IActionResult> Create(OdstavkyViewModel odstavky)
        {
            var lokalitaSearch = await _context.LokalityS
            .Include(l => l.Region)
            .ThenInclude(r => r.Firma)
            .FirstOrDefaultAsync(input => input.Lokalita == odstavky.AddOdstavka.Lokality.Lokalita);


            return await HandleOdstavkyDieslovani(lokalitaSearch, odstavky.AddOdstavka.Od, odstavky.AddOdstavka.Do, odstavky, odstavky.AddOdstavka.Popis);
        }
        private async Task<TableLokality?> GetLokalitaAsync(OdstavkyViewModel odstavky)
        {
            return await _context.LokalityS
                .Include(l => l.Region)
                .ThenInclude(r => r.Firma)
                .FirstOrDefaultAsync(input => input.Lokalita == odstavky.AddOdstavka.Lokality.Lokalita);

        }

        private async Task<TableDieslovani?> GetHigherPriortiy(TableOdstavky newOdstavka, OdstavkyViewModel odstavky)
        {
            var dieslovani = await _context.DieslovaniS
            .Include(o => o.Odstavka)
            .ThenInclude(o => o.Lokality)
            .Include(o => o.Firma)
            .Include(o => o.Technik)
            .ThenInclude(o => o.Firma)
            .Include(o => o.Firma)
            .Where(p =>
            p.Technik.Firma.IDFirmy == newOdstavka.Lokality.Region.Firma.IDFirmy &&
            p.Technik.Taken == true).FirstAsync();

            if (dieslovani == null)
            {
                Zpravy.Add("Dieslovani nenalezeno ");
                return null;
            }
            else
            {
                int staraVaha = dieslovani.Odstavka.Lokality.Klasifikace.ZiskejVahu();
                int novaVaha = newOdstavka.Lokality.Klasifikace.ZiskejVahu();

                bool maVyssiPrioritu = novaVaha > staraVaha;
                bool casovyLimit = dieslovani.Odstavka.Od.Date.AddHours(2) < DateTime.Now;
                bool daPodminka = dieslovani.Odstavka.Lokality.DA == "FALSE";

                if (maVyssiPrioritu && casovyLimit && daPodminka)
                {
                    Zpravy.Add("Podminka pro prioritu splněna");
                    var novyTechnik = await _context.TechniS.FirstOrDefaultAsync(p => p.IdTechnika == "606794464");
                    if (novyTechnik != null)
                    {
                        dieslovani.Technik = novyTechnik;
                        _context.DieslovaniS.Update(dieslovani);
                        await _context.SaveChangesAsync();
                        Zpravy.Add("Přiřazen fiktivni technik na lokalitu" + newOdstavka.Lokality.Lokalita);

                    }
                    return dieslovani;
                }
                else
                {

                    var novyTechnik = await _context.TechniS.Where(t=>t.IdTechnika=="606794494").FirstOrDefaultAsync();
                    
                    if(novyTechnik==null){Zpravy.Add("Pokus o přivázání nového fiktivního technika"); return null;}

                    var NewDieslovani = await CreateNewDielovaniAsync(newOdstavka, novyTechnik, novyTechnik.Firma, odstavky);  return NewDieslovani;
                    
                }
            }
      


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

        public string GetTechnikLokalita(string technikId)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return _context.DieslovaniS
            .Where(d => d.IdTechnik == technikId)
            .Select(d => d.Odstavka.Lokality.Lokalita)
            .FirstOrDefault();
#pragma warning restore CS8603 // Possible null reference return.
        }

        private TableOdstavky CreateNewOdstavka(OdstavkyViewModel odstavky, TableLokality lokalitaSearch, string distrib, DateTime od, DateTime do_, string popis)
        {
            return new TableOdstavky
            {
                Distributor = distrib,
                Firma = lokalitaSearch.Region.Firma.NázevFirmy,
                Od = od,
                Do = do_,
                Vstup = odstavky.AddOdstavka.Vstup,
                Odchod = odstavky.AddOdstavka.Odchod,
                Popis = popis,
                LokalitaId = lokalitaSearch.Id
            };
        }
        private bool ISvalidDateRange(DateTime od, DateTime Do)
        {
            return od.Date >= DateTime.Today && od < Do;
        }
        private bool ExistingOdstavka(int lokalitaSearchId, DateTime od)
        {
            var existingOdstavka = _context.OdstavkyS
            .FirstOrDefault(o => o.Od == od && o.Lokality.Id == lokalitaSearchId);
            if (existingOdstavka == null) { return true; }
            else { return false; }
        }
        private async Task<TableTechnici?> AssignTechnikAsync(TableOdstavky newOdstavka, TableLokality lokalitaSearch, OdstavkyViewModel odstavky)
        {
            var firmaVRegionu = await GetFirmaVRegionuAsync(lokalitaSearch.Region.IdRegion);

            Zpravy.Add(firmaVRegionu.NázevFirmy);
            if (firmaVRegionu != null)
            {
                var technikSearch = await _context.Pohotovts
                .Include(p => p.Technik.Firma)
                .Where(p => p.Technik.FirmaId == firmaVRegionu.IDFirmy && p.Technik.Taken == false)
                .Select(p => p.Technik)
                .FirstOrDefaultAsync();



                if (technikSearch == null) //žádný technik není volný
                {

                    technikSearch = await CheckTechnikReplacementAsync(newOdstavka, firmaVRegionu, odstavky); //

                    if (technikSearch != null)
                    {
                        await CreateNewDielovaniAsync(newOdstavka, technikSearch, firmaVRegionu, odstavky);
                        return technikSearch;
                    }
                    else
                    {
                        Zpravy.Add("Žádný náhradní technik nebyl nalezen.");
                        return technikSearch;
                    }


                }

                if (technikSearch != null)
                {
                    Zpravy.Add(technikSearch.Jmeno);
                    await CreateNewDielovaniAsync(newOdstavka, technikSearch, firmaVRegionu, odstavky);

                    return technikSearch;

                }
            }
            Zpravy.Add("Firma V regionu je null");

            return null;

        }
        private async Task<TableFirma?> GetFirmaVRegionuAsync(int regionId)
        {
            return await _context.ReginoS
            .Where(r => r.IdRegion == regionId)
                .Select(r => r.Firma)
                .FirstOrDefaultAsync();
        }
        private async Task<TableTechnici?> CheckTechnikReplacementAsync(TableOdstavky newOdstavka, TableFirma firmaVRegionu, OdstavkyViewModel odstavky)
        {
            var dieslovani = await GetHigherPriortiy(newOdstavka, odstavky);
            if (dieslovani == null) { Zpravy.Add("Priority je null."); return null; }
            else
            {
                return dieslovani.Technik;
            }

        }
        private async Task<TableDieslovani> CreateNewDielovaniAsync(TableOdstavky newOdstavky, TableTechnici technik, TableFirma firmaVRegionu, OdstavkyViewModel odstavky)
        {
            var NewDieslovani = new TableDieslovani
            {
                Vstup = odstavky.DieslovaniMod.Vstup,
                Odchod = odstavky.DieslovaniMod.Odchod,
                IDodstavky = newOdstavky.IdOdstavky,
                IdTechnik = technik.IdTechnika,
                FirmaId = firmaVRegionu.IDFirmy
            };
            _context.DieslovaniS.Add(NewDieslovani);
            technik.Taken = true;
            _context.TechniS.Update(technik);
            await _context.SaveChangesAsync();

            return NewDieslovani;
        }
    



        public async Task<IActionResult> Vstup(OdstavkyViewModel odstavky)
        {
            var SetOdstavka = new TableOdstavky
            {
                Vstup = odstavky.AddOdstavka.Vstup

            };
            _context.OdstavkyS.Add(SetOdstavka);
            await _context.SaveChangesAsync();
            return Redirect("/Home/Index");

        }


        public async Task<IActionResult> Test(OdstavkyViewModel odstavky)
        {
            for (int i = 1; i <= 1; i++)
            {
                try
                {
                    var number = await _context.LokalityS.CountAsync();
                    if (number == 0)
                    {
                        return Json(new { success = false, message = "Chyba při zakládání." });
                    }

                    var IdNumber = RandomNumberGenerator.GetInt32(1, number);
                    var lokalitaSearch = await _context.LokalityS
                    .Include(o => o.Region)
                    .ThenInclude(p => p.Firma)
                    .FirstOrDefaultAsync(i => i.Id == IdNumber);

                    if (lokalitaSearch != null)
                    {
                    var hours = RandomNumberGenerator.GetInt32(1, 50);
                    string popis = "test";

                    return await HandleOdstavkyDieslovani(
                    lokalitaSearch,
                    DateTime.Today.AddHours(hours + 2),
                    DateTime.Today.AddHours(hours + 8),
                    odstavky,
                    popis
                    );
                    }

                    return Json(new { success = true, message = "Dieslování objednáno." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Chyba při objednání dieslování: " + ex.Message });
                }


            }
            return Redirect("/Home/Index");
        }
        public async Task<IActionResult> Odchod(OdstavkyViewModel odstavky)
        {
            var SetOdstavka = new TableOdstavky
            {
                Odchod = odstavky.AddOdstavka.Odchod
            };
            _context.OdstavkyS.Add(SetOdstavka);
            await _context.SaveChangesAsync();
            return Redirect("/Home/Index");
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

         public async Task<IActionResult> GetTableData(int start = 0, int length = 0)
        {
            // Celkový počet záznamů v tabulce
            int totalRecords = await _context.OdstavkyS.CountAsync();
            length = totalRecords;
            // Načtení záznamů pro aktuální stránku
            var LokalityList = await _context.OdstavkyS
                .Include(o=>o.Lokality)
                .OrderBy(o => o.IdOdstavky) // Nebo jiný řadící sloupec
                .Skip(start)
                .Take(length)
                .Select(l => new
                {
                    l.IdOdstavky,
                    l.Distributor,
                    l.Lokality.Lokalita,
                    l.Lokality.Klasifikace,
                    l.Od,
                    l.Do,
                    l.Lokality.Adresa,
                    l.Lokality.Baterie,
                    l.Popis,
                    l.Lokality.Zásuvka,
                    EmptyColumn = (string)null  

                    
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