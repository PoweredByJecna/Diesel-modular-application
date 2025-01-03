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
        public async Task<TableDieslovani> HandleOdstavkyDieslovani(TableLokality lokalitaSearch, DateTime od, DateTime do_, OdstavkyViewModel odstavky, string popis)
        {
            try
            {  
                Zpravy.Add("<br>Od: " + od);
                Zpravy.Add("<br>Do: " + do_);
                string distrib = DetermineDistributor(lokalitaSearch.Region.NazevRegionu);
                Debug.WriteLine($"Distributor: {distrib}");

                if (!ExistingOdstavka(lokalitaSearch.Id, od))
                {
                    return null;
                }
                else
                {
                    Debug.WriteLine($"není jiná odstávka");
                    if (!ISvalidDateRange(od, do_))
                    {
                        Debug.WriteLine($"špatně zadané datum");
                        return null;
                    }
                    else
                    {
                        var newOdstavka = CreateNewOdstavka(odstavky, lokalitaSearch, distrib, od, do_, popis);
                        try
                        {
                            _context.Add(newOdstavka);
                            await _context.SaveChangesAsync();
                            Zpravy.Add("<br>Jiná odstávka nenalezena.");
                            Zpravy.Add("<br>Založená odstávka: " + newOdstavka.IdOdstavky);
                            Debug.WriteLine($"Id odstavky: {newOdstavka.IdOdstavky}");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Chyba při ukládání do databáze: {ex.Message}");
                        }
                        if(newOdstavka.Lokality.DA=="TRUE")
                        {
                            Zpravy.Add("<br>Na lokalitě je DA");
                            Debug.WriteLine($"Na lokalitě se nachází stacionární generátor {newOdstavka.Lokality.Lokalita}");

                            return null;
                        }
                        if(IsDieselRequired(newOdstavka.Lokality.Klasifikace,newOdstavka.Od, newOdstavka.Do, newOdstavka.Lokality.Baterie))
                        {
                            var technikSearch = await AssignTechnikAsync(newOdstavka); 
                            if (technikSearch == null)
                            {
                                Zpravy.Add("<br>Něco je špatně"); 
                                return null;
                            }
                            else
                            {
                                Zpravy.Add("<br> Dieslovani vytvořeno");
                                var dieslovani = await _dieslovani.CreateNewDieslovaniAsync(newOdstavka, technikSearch);
                                return dieslovani;
                            }
                        }
                        else
                        {
                           Zpravy.Add("<br>Dielsovani neni potřeba");
                           return null;

                        }
                    }
                }
            }
            catch
            {
                return null;;
            }
        }
        public async Task<TableDieslovani> Create(OdstavkyViewModel odstavky)
        {
            var lokalitaSearch = await _context.LokalityS
            .Include(l => l.Region)
            .ThenInclude(r => r.Firma)
            .FirstOrDefaultAsync(input => input.Lokalita == odstavky.AddOdstavka.Lokality.Lokalita);


            var dieslovani = await HandleOdstavkyDieslovani(lokalitaSearch, odstavky.AddOdstavka.Od, odstavky.AddOdstavka.Do, odstavky, odstavky.AddOdstavka.Popis);
            return dieslovani;
        }
        private async Task<TableLokality?> GetLokalitaAsync(OdstavkyViewModel odstavky)
        {
            return await _context.LokalityS
                .Include(l => l.Region)
                .ThenInclude(r => r.Firma)
                .FirstOrDefaultAsync(input => input.Lokalita == odstavky.AddOdstavka.Lokality.Lokalita);

        }

        private async Task<TableTechnici?> GetHigherPriority(TableOdstavky newOdstavka)
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
            p.Technik.Taken == true).FirstOrDefaultAsync();

            if (dieslovani == null)
            {
                Debug.WriteLine($"Dieslovani nenalezeno");

                
                return null;
            }
            else
            {

                if(dieslovani.Odstavka.Do<newOdstavka.Od.AddHours(3) || newOdstavka.Do<dieslovani.Odstavka.Od.AddHours(3))
                {
                    Zpravy.Add("<br>Technikovi: " + dieslovani.Technik.Jmeno + "bylo přiřazeno další dieslovani");
                    return dieslovani.Technik;
                }
                else
                {
                    int staraVaha = dieslovani.Odstavka.Lokality.Klasifikace.ZiskejVahu();
                    int novaVaha = newOdstavka.Lokality.Klasifikace.ZiskejVahu();

                    bool maVyssiPrioritu = novaVaha > staraVaha;
                    bool casovyLimit = dieslovani.Odstavka.Od.Date.AddHours(3) < DateTime.Now;
                    bool daPodminka = dieslovani.Odstavka.Lokality.DA == "FALSE";

                    if (maVyssiPrioritu && casovyLimit && daPodminka)
                    {
                        Debug.WriteLine($"Podminka pro prioritu splněna");
                        var novyTechnik = await _context.TechniS.FirstOrDefaultAsync(p => p.IdTechnika == "606794494");
                        if (novyTechnik != null)
                        {
                            await _dieslovani.CreateNewDieslovaniAsync(newOdstavka,dieslovani.Technik);
                            dieslovani.Technik = novyTechnik;
                            _context.DieslovaniS.Update(dieslovani);
                            await _context.SaveChangesAsync();
                            Zpravy.Add("<br>Fiktivní technik byl přiřazen staré lokalitě: "+ dieslovani.Odstavka.Lokality.Lokalita);
                            Debug.WriteLine($"Přiřazen fiktivni technik na lokalitu: {dieslovani.Odstavka.Lokality.Lokalita}");
                        }
                        return novyTechnik;
                    }
                    else
                    {

                        var novyTechnik = await _context.TechniS.Where(t=>t.IdTechnika=="606794494").FirstOrDefaultAsync();
                        
                        if(novyTechnik==null)
                        {
                            return null;
                        }
                        return novyTechnik;
                        
                    }
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
            Debug.WriteLine($"vytváří se odstávka s parametry: {distrib }, {lokalitaSearch.Region.Firma.NázevFirmy}, {od}, {do_},{odstavky.AddOdstavka.Vstup}, {odstavky.AddOdstavka.Odchod}, {popis}, {lokalitaSearch.Id} ");
            return new TableOdstavky
            {
                Distributor = distrib,
                Firma = lokalitaSearch.Region.Firma.NázevFirmy,
                Od = od,
                Do = do_,
                Vstup = DateTime.MinValue,
                Odchod = DateTime.MinValue,
                Popis = popis,
                LokalitaId = lokalitaSearch.Id   
            };
            
        }
        private bool Battery(DateTime od, DateTime Do, string baterie)
        {

            var rozdil = (Do - od).TotalMinutes;

            if (rozdil > Convert.ToInt32(baterie))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool IsDieselRequired(string Klasifikace, DateTime Od, DateTime Do, string Baterie)
        {
            var CasVypadku = Klasifikace.ZiskejCasVypadku();
            var rozdil = (Do - Od).TotalMinutes;

            if(CasVypadku*60>rozdil)
            {
                Zpravy.Add("<br>Lokalita s klasifikací: " + Klasifikace + " může být 12h dole");
                return false;
            }
            else
            {
                if(Battery(Od, Do, Baterie))
                {
                    Zpravy.Add("<br>Baterie vydrží: " + Baterie + " min, čas doby odstávky je: " + (Do-Od).TotalMinutes + " min");
                    return false;
                }
                else
                {
                    Zpravy.Add("<br>Dieslovaní je potřeba");
                    return true;
                }
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
        private async Task<TableTechnici?> AssignTechnikAsync(TableOdstavky newOdstavka)
        {
            var firmaVRegionu = await GetFirmaVRegionuAsync(newOdstavka.Lokality.Region.IdRegion);
            if(firmaVRegionu !=null)
            {
                Zpravy.Add("<br>Firma: " + firmaVRegionu.NázevFirmy);
                Debug.WriteLine($"Vybraná firma: {firmaVRegionu.NázevFirmy}");

                var technikSearch = await _context.Pohotovts
                .Include(p => p.Technik.Firma)
                .Where(p => p.Technik.FirmaId == firmaVRegionu.IDFirmy && p.Technik.Taken == false)
                .Select(p => p.Technik)
                .FirstOrDefaultAsync();

                if (technikSearch == null) //žádný technik není volný nebo nemá pohotovost
                {
                    Zpravy.Add("<br>Technici jsou obsazeni<br>,nebo nemají pohotovost");

                    if(_context.Pohotovts.Include(p => p.Technik.Firma).Where(p => p.Technik.FirmaId == firmaVRegionu.IDFirmy).Any()) //kontrola zda ma nějaky technik vubec pohotovost
                    {
                        Zpravy.Add("<br>alespon jeden technik v regionu pohotovost ma, zkus nahradit.");
                        Debug.WriteLine($"alespon jeden technik v regionu pohotovost ma, zkus nahradit.");
                        technikSearch = await CheckTechnikReplacementAsync(newOdstavka); //alespon jeden technik v regionu pohotovost ma, zkus nahradit
                       
                        
                        if (technikSearch != null) //technik nahrazen
                        {
                            Zpravy.Add("<br>technik: " + technikSearch.Jmeno + " je nahrazen");
                            Debug.WriteLine($"Technik: {technikSearch.Jmeno}, je nahrazen");
                            return technikSearch;
                        }
                        else
                        {
                            Zpravy.Add("<br>Žádný náhradní technik nebyl nalezen");
                            Debug.WriteLine($"Žádný náhradní technik nebyl nalezen");
                            return technikSearch;
                        }

                    }
                    else
                    {
                        Zpravy.Add("<br>Žádný technik nemá pohotovost, <br>Je přiřazen fiktivní");
                        Debug.WriteLine($"Žádný technik nemá pohotovost v daném regionu, bude přiřazen fiktivní");
                        var novyTechnik = await _context.TechniS.FirstOrDefaultAsync(p => p.IdTechnika == "606794494");
                        if(novyTechnik!=null)
                        {
                            await _dieslovani.CreateNewDieslovaniAsync(newOdstavka,novyTechnik);
                        }
                        return novyTechnik;
                    
                    }

                }

                if (technikSearch != null)
                {
                    Debug.WriteLine($"Technik: {technikSearch.Jmeno}, je zapsán a pojede dieslovat");
                    return technikSearch;

                }
                else
                {
                    Zpravy.Add("<br>1");
                    return null;
                }

                   

            }
            else
            {
                Zpravy.Add("<br>2");
                return null;
            }
         

        }
        private async Task<TableFirma?> GetFirmaVRegionuAsync(int regionId)
        {
            return await _context.ReginoS
            .Where(r => r.IdRegion == regionId)
                .Select(r => r.Firma)
                .FirstOrDefaultAsync();

            
        }
        private async Task<TableTechnici?> CheckTechnikReplacementAsync(TableOdstavky newOdstavka)
        {
            var technik = await GetHigherPriority(newOdstavka);
            if (technik == null) 
            { 
                
                Debug.WriteLine($"Priority je null.");
                return null; 
            }
            else
            {
                Debug.WriteLine($"Technik byl nalezen, nebo nahrazen");
                return technik;
            }

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
    try
    {
        var number = await _context.LokalityS.CountAsync();
        if (number == 0)
        {
            TempData["Zprava"] = "Chyba při zakládání.";
            return Redirect("/Home/Index"); 
        }

        var IdNumber = RandomNumberGenerator.GetInt32(1, number);
        var lokalitaSearch = await _context.LokalityS
            .Include(o => o.Region)
            .ThenInclude(p => p.Firma)
            .FirstOrDefaultAsync(i => i.Id == IdNumber);


        if (lokalitaSearch != null)
        {
            Zpravy.Add("<br>Lokalita: "+ lokalitaSearch.Lokalita);
            Debug.WriteLine($"Vybraná lokalita: {lokalitaSearch.Lokalita}, Region: {lokalitaSearch.Region.NazevRegionu}");

            var hours = RandomNumberGenerator.GetInt32(1, 50);
            string popis = "test";

            var result = await HandleOdstavkyDieslovani(lokalitaSearch, DateTime.Today.AddHours(hours + 2), DateTime.Today.AddHours(hours + 8), odstavky, popis);

            Zpravy.Add("<br>Result: " + result);

            if (result == null)
            {
                TempData["Zprava"] = "Založení odstávky nebo dieslovaní selhalo z důvodu:<br> " + string.Join("", Zpravy);
                return Redirect("/Home/Index"); 
            }
            else
            {
                TempData["Zprava"] = "Dieslování objednáno:<br>" + string.Join("", Zpravy); 
                return Redirect("/Home/Index");
            }
            
        }
        else
        {
            TempData["Zprava"] = "Lokalita nenalezena" + string.Join("", Zpravy); 
            return Redirect("/Home/Index");


        }

        

    }
    catch (Exception ex)
    {
        TempData["Zprava"] = "Chyba při objednání dieslování " + ex.Message;
        odstavky.Barva="red";
        return Redirect("/Home/Index"); 
    }
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