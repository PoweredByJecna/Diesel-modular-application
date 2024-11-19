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

namespace Diesel_modular_application.Controllers
{
    public class OdstavkyController : Controller
    {
        private readonly DAdatabase _context;

        public OdstavkyController(DAdatabase context)
        {
            _context = context;
        }

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

            odstavky.CurrentPage = page;
            odstavky.TotalPages = (int)Math.Ceiling(await odstavkyQuery.CountAsync() / (double)pagesize);
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
                    .ToListAsync();
                search.OdstavkyList = FilteredList;
            }
            return PartialView("_OdstavkyListPartial", search);
        }
        public async Task<IActionResult> HandleOdstavkyDieslovani(TableLokality lokalitaSearch, DateTime od, DateTime do_, OdstavkyViewModel odstavky,string popis)
        {
            if (lokalitaSearch == null)
            {
                ViewBag.Message = "Zadaná lokalita neexistuje";
                return View("Index", odstavky);
            }

            string distrib = DetermineDistributor(lokalitaSearch.Region.NazevRegionu);
            var newOdstavka = CreateNewOdstavka(odstavky, lokalitaSearch, distrib, od, do_, popis);

            if(ExistingOdstavka(newOdstavka.LokalitaId,newOdstavka.Do)){}
            else
            {
                TempData["Zprava"] = "Odstávka na tento den: " + newOdstavka.Od  +" lokalitu: " + newOdstavka.LokalitaId + "(Id)";  
                return Redirect("/Home/Index");
            }

            if (!ISvalidDateRange(newOdstavka.Od, newOdstavka.Do))
            {
                TempData["Zprava"] = "Špatně zadané datum";
                return Redirect("/Home/Odstavky");
            }
            else
            {
                await _context.OdstavkyS.AddAsync(newOdstavka);
                await _context.SaveChangesAsync();
            }

            var technikSearch = await AssignTechnikAsync(newOdstavka, lokalitaSearch, odstavky);
            
            if (technikSearch == null)
            {
                    
                return Redirect("/Home/Index");
            }
            else
            {
                TempData["Zprava"] = "Technik: " + technikSearch.Jmeno + " je objednán na dieslovaní";
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
        private async Task<TableDieslovani?> GetHigherPriortiy(TableOdstavky newOdstavka)
        {
            var dieslovani = await _context.Pohotovts
            .Where(p => p.Technik.FirmaId == newOdstavka.Lokality.Region.IdRegion && p.Technik.Taken == true)
            .SelectMany(p => _context.DieslovaniS
            .Where(td => td.Technik.IdTechnika == p.Technik.IdTechnika)
            .Include(td => td.Odstavka).ThenInclude(td => td.Lokality))
            .FirstOrDefaultAsync();

            if (dieslovani == null)
            {
                return null;
            }

            int staraVaha = dieslovani.Odstavka.Lokality.Klasifikace.ZiskejVahu();
            int novaVaha = newOdstavka.Lokality.Klasifikace.ZiskejVahu();

            bool maVyssiPrioritu = novaVaha > staraVaha;
            bool casovyLimit = dieslovani.Odstavka.Od.Date.AddHours(2) < DateTime.Now;
            bool daPodminka = dieslovani.Odstavka.Lokality.DA == "FALSE";

            if(maVyssiPrioritu && casovyLimit && daPodminka)
            {
                return dieslovani;
            }
            else
            {
                var novyTechnik = await _context.TechniS.FirstOrDefaultAsync(p => p.IdTechnika == "606794464");
                if (novyTechnik != null)
                {
                    dieslovani.Technik = novyTechnik;
                    _context.DieslovaniS.Update(dieslovani);
                    await _context.SaveChangesAsync();
                }
            }

            return null;


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
            if(existingOdstavka==null){return true;}
            else{return false;}
        }
        private async Task<TableTechnici?> AssignTechnikAsync(TableOdstavky newOdstavka, TableLokality lokalitaSearch, OdstavkyViewModel odstavky)
        {
            var firmaVRegionu = await GetFirmaVRegionuAsync(lokalitaSearch.Region.IdRegion);
            if(firmaVRegionu!=null)
            {
                var technikSearch = await _context.Pohotovts
                .Where(p => p.Technik.FirmaId == firmaVRegionu.IDFirmy && !p.Technik.Taken)
                .Select(p => p.Technik)
                .FirstOrDefaultAsync();

                if (technikSearch == null)
                {
                    technikSearch = await CheckTechnikReplacementAsync(newOdstavka, firmaVRegionu, odstavky);

                    if(technikSearch!=null)
                    { 
                        await CreateNewDielovaniAsync(newOdstavka, technikSearch, firmaVRegionu, odstavky);
                       return technikSearch;
                    }
                    else
                    {
                        TempData["Zprava"] = "Žádný náhradní technik nebyl nalezen.";
                        return technikSearch;
                    }


                }

                if (technikSearch != null)
                {
                    await CreateNewDielovaniAsync(newOdstavka, technikSearch, firmaVRegionu, odstavky);
                    return technikSearch;

                }
            }
            TempData["Zprava"] = "Technik nemá pohotovost.";

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
            var dieslovani = await GetHigherPriortiy(newOdstavka);
            if (dieslovani == null)  {TempData["Zprava"] = "Priority je null."; return null;}
            else
            {
                return dieslovani.Technik;
            }

        }
        private async Task CreateNewDielovaniAsync(TableOdstavky newOdstavky, TableTechnici technik, TableFirma firmaVRegionu, OdstavkyViewModel odstavky)
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
            await _context.SaveChangesAsync();

            if (odstavky.AddOdstavka.Od.Date == DateTime.Today)
            {
            technik.Taken = true;
            _context.TechniS.Update(technik);
            }
            await _context.SaveChangesAsync();
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
            for (int i = 1; i <= 10; i++)
            {
                var number = await _context.LokalityS.CountAsync();
                var IdNumber = RandomNumberGenerator.GetInt32(1, number);
                var lokalitaSearch = await _context.LokalityS.Include(o => o.Region).ThenInclude(p => p.Firma).FirstOrDefaultAsync(i => i.Id == IdNumber);
                if (lokalitaSearch!=null)
                {
                    var hours = RandomNumberGenerator.GetInt32(1, 50);
                    string popis = "test";
                    return await HandleOdstavkyDieslovani(lokalitaSearch, DateTime.Today.AddHours(hours+2), DateTime.Today.AddHours(hours+8), odstavky, popis);
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


        public async Task<IActionResult> Delete(OdstavkyViewModel odstavky)
        {

            var odstavka = await _context.OdstavkyS.FindAsync(odstavky.OdstavkyMod.IdOdstavky);
            if (odstavka != null)
            {
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

            }

            _context.OdstavkyS.Remove(odstavka);
            await _context.SaveChangesAsync();
            return Redirect("/Odstavky/Index");
        }



    }

}