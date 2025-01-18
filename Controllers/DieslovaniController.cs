using System.Diagnostics;
using Diesel_modular_application.Data;
using Diesel_modular_application.KlasifikaceRule;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Diesel_modular_application.Controllers.OdstavkyController;

namespace Diesel_modular_application.Controllers
{
    public class DieslovaniController:Controller
    {
        private readonly DAdatabase _context;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly TableOdstavky _odstavky;




        

        public DieslovaniController(DAdatabase context, UserManager<IdentityUser> userManager, TableOdstavky odstavky)
        {
            _context = context;
            _userManager = userManager;
            _odstavky = odstavky;

        }
        [Authorize]
        public IActionResult IndexAsync()
        {
            return View();    
        }

        public async Task<IActionResult> DetailDieslovani(int id)
        {
            var detail = await _context.DieslovaniS
                .Include(o => o.Odstavka)
                .ThenInclude(o => o.Lokality)
                .ThenInclude(o => o.Region)
                .Include(p => p.Technik)
                .FirstOrDefaultAsync(o => o.IdDieslovani == id);

            var odstavky = new OdstavkyViewModel
            {
                DieslovaniMod = detail,
            };

            return View(odstavky);
        }

        public async Task<HandleOdstavkyDieslovaniResult> HandleOdstavkyDieslovani(TableLokality lokalitaSearch, DateTime od, DateTime do_, OdstavkyViewModel odstavky, string popis, TableOdstavky newOdstavka, HandleOdstavkyDieslovaniResult result)
        {
           
            
            if(newOdstavka.Lokality.DA==true)
            {
                Debug.WriteLine($"Na lokalitě je DA");
                result.Success = false;
                result.Message = "Na lokalitě se nachází stacionární generátor.";
                return result;
            }
            if(newOdstavka.Lokality.Zasuvka==false)
            {
                Debug.WriteLine($"Na lokalitě neni zasuvka");
                result.Success = false;
                result.Message = "Na lokalitě se není zásuvka.";
                return result;
            }
            if(IsDieselRequired(newOdstavka.Lokality.Klasifikace,newOdstavka.Od, newOdstavka.Do, newOdstavka.Lokality.Baterie))
            {
                var technikSearch = await AssignTechnikAsync(newOdstavka); 

                if (technikSearch == null)
                {
                    result.Success = false;
                    Debug.WriteLine($"Nepodařilo se najít technika");
                    result.Message = "Nepodařilo se přiřadit technika.";
                    return result;
                }
                else
                {
                    var dieslovani = await CreateNewDieslovaniAsync(newOdstavka, technikSearch);
                    result.Dieslovani = dieslovani;
                    result.Message = "Dieslování bylo úspěšně vytvořeno.";
                }
            }
            else
            {
                result.Success = false;
                result.Message = "Dieslovani neni potřeba";
                Debug.WriteLine($"Dislovani neni potřeba");
                return result;

            }
            
            result.Success = true;
            return result;
          
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
                    return dieslovani.Technik;
                }
                else
                {
                    int staraVaha = dieslovani.Odstavka.Lokality.Klasifikace.ZiskejVahu();
                    int novaVaha = newOdstavka.Lokality.Klasifikace.ZiskejVahu();

                    bool maVyssiPrioritu = novaVaha > staraVaha;
                    bool casovyLimit = dieslovani.Odstavka.Od.Date.AddHours(3) < DateTime.Now;
                    bool daPodminka = dieslovani.Odstavka.Lokality.DA == false;

                    if (maVyssiPrioritu && casovyLimit && daPodminka)
                    {
                        Debug.WriteLine($"Podminka pro prioritu splněna");
                        var novyTechnik = await _context.TechniS.FirstOrDefaultAsync(p => p.IdTechnika == "606794494");
                        if (novyTechnik != null)
                        {
                            await CreateNewDieslovaniAsync(newOdstavka,dieslovani.Technik);
                            dieslovani.Technik = novyTechnik;
                            _context.DieslovaniS.Update(dieslovani);
                            await _context.SaveChangesAsync();
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
        private bool IsDieselRequired(string Klasifikace, DateTime Od, DateTime Do, string Baterie)
        {
            var CasVypadku = Klasifikace.ZiskejCasVypadku();
            var rozdil = (Do - Od).TotalMinutes;

            if(CasVypadku*60>rozdil)
            {
                Debug.WriteLine($"Lokalita je: {Klasifikace} může být 12h dole" );
                return false;
            }
            else
            {
                if(Battery(Od, Do, Baterie))
                {
                    Debug.WriteLine($"Baterie vydrží:  {Baterie } min, čas doby odstávky je:" + (Do-Od).TotalMinutes + "min" );
                    return false;
                }
                else
                {
                    Debug.WriteLine($"Dislovani je potřeba" );
                    return true;
                }
            }
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
 

    
        private async Task<TableTechnici?> AssignTechnikAsync(TableOdstavky newOdstavka)
        {
            var firmaVRegionu = await GetFirmaVRegionuAsync(newOdstavka.Lokality.Region.IdRegion);
            if(firmaVRegionu !=null)
            {
                Debug.WriteLine($"Vybraná firma: {firmaVRegionu.NázevFirmy}");

                var technikSearch = await _context.Pohotovts
                .Include(p => p.Technik.Firma)
                .Where(p => p.Technik.FirmaId == firmaVRegionu.IDFirmy && p.Technik.Taken == false)
                .Select(p => p.Technik)
                .FirstOrDefaultAsync();

                if (technikSearch == null) //žádný technik není volný nebo nemá pohotovost
                {
                    Debug.WriteLine($"Technici jsou obsazeni,nebo nemají pohotovost");


                    if(_context.Pohotovts.Include(p => p.Technik.Firma).Where(p => p.Technik.FirmaId == firmaVRegionu.IDFirmy).Any()) //kontrola zda ma nějaky technik vubec pohotovost
                    {
                        Debug.WriteLine($"alespon jeden technik v regionu pohotovost ma, zkus nahradit.");
                        technikSearch = await CheckTechnikReplacementAsync(newOdstavka); //alespon jeden technik v regionu pohotovost ma, zkus nahradit
                       
                        
                        if (technikSearch != null) //technik nahrazen
                        {
                            Debug.WriteLine($"Technik: {technikSearch.Jmeno}, je nahrazen");
                            return technikSearch;
                        }
                      

                    }
                    Debug.WriteLine($"Žádný technik nebyl nalezen, bude přiřazen fiktivní");
                    var fiktivniTechnik = await _context.TechniS.FirstOrDefaultAsync(p => p.IdTechnika == "606794494");

                   

                    return fiktivniTechnik;

                }

                if (technikSearch != null)
                {
                    Debug.WriteLine($"Technik: {technikSearch.Jmeno}, je zapsán a pojede dieslovat");
                    return technikSearch;

                }
                else
                {
                  
                    return technikSearch;
                }

                   

            }
            else
            {
                
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

        public async Task<IActionResult> Vstup (int IdDieslovani)
        {
            try
            {
                var dis = await _context.DieslovaniS
                .Include(d => d.Technik).Include(d=>d.Odstavka)  // Zajišťuje načtení spojeného technika
                .FirstAsync(d=>d.IdDieslovani==IdDieslovani);
                if(dis !=null)
                {    
                    dis.Vstup=DateTime.Now;
                    dis.Technik.Taken=true;
                    _context.DieslovaniS.Update(dis);
                    var odstavka = await _context.OdstavkyS.FindAsync(dis.Odstavka.IdOdstavky);
                    if (odstavka != null)
                    {
                        odstavka.ZadanVstup = true;  
                        _context.OdstavkyS.Update(odstavka);
                    }   
                }
                await _context.SaveChangesAsync();
                return Json(new
                {
                    success = true,
                    message = "Byl zadán vstup na lokalitu.",
                    tempMessage = TempData["Zprava"]  // Return the message for the modal
                });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = "Chyba při zadávání vstupu " + ex.Message });
            }
        }
        public async Task<IActionResult> Odchod (int IdDieslovani)
        {
           try
            {
                var dis = await _context.DieslovaniS
                .Include(d => d.Technik)
                .Include(d=>d.Odstavka)
                .FirstAsync(d=>d.IdDieslovani==IdDieslovani);
                if(dis !=null)
                {    
                    var odstavka = await _context.OdstavkyS.FindAsync(dis.Odstavka.IdOdstavky);
                    if (odstavka != null)
                    {
                        odstavka.ZadanOdchod=true;
                        odstavka.ZadanVstup=false;
                        _context.Update(odstavka);
                    }   
                    var anotherDiesel = await _context.DieslovaniS.Include(o=>o.Odstavka).Include(o => o.Technik).Where(o => o.Technik.IdTechnika == dis.Technik.IdTechnika && o.Odstavka.ZadanOdchod==false).FirstOrDefaultAsync();
                    if(anotherDiesel==null)
                    {

                        dis.Technik.Taken=false;
                    }
                    dis.Odchod=DateTime.Now;
                    _context.Update(dis);
                }
                 
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Byl zadán odchod z lokality." });
            }
           catch (Exception ex)
            {
                return Json(new { success = false, message = "Chyba při zadávání odchodu " + ex.Message });
            }

            
        }
        




        public async Task<IActionResult> TemporaryLeave (OdstavkyViewModel dieslovani)
        {
             var dis = await _context.DieslovaniS
            .Include(d => d.Technik)  // Zajišťuje načtení spojeného technika
            .FirstAsync(d=>d.IdDieslovani==dieslovani.DieslovaniMod.IdDieslovani);
            if(dis.Technik.Taken)
            {
                dis.Technik.Taken=false;
                _context.Update(dis);
            }
            else
            {
                dis.Technik.Taken=true;
                _context.Update(dis);
            }
            await _context.SaveChangesAsync();
            return Redirect ("/Dieslovani/Index");
        }
        public async Task<IActionResult> Take(int IdDieslovani)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var technik = await _context.TechniS.FirstAsync(d=>d.IdUser==currentUser.Id);
                var dieslovaniTaken = await _context.DieslovaniS
               
                .Include(d=>d.Technik)
                .FirstOrDefaultAsync(d=>d.IdDieslovani==IdDieslovani);

                if(dieslovaniTaken!=null)
                {
                    if(technik.Taken==true)
                    {
                        TempData["Zprava"] = "Již máte dieslovaní převzaté";
                    }

                    dieslovaniTaken.Technik=technik;
                    technik.Taken=true;
                    _context.Update(technik);
                    _context.Update(dieslovaniTaken);

                    await _context.SaveChangesAsync();
                    
                    TempData["Zprava"] = "Dieslování bylo úspěšně zadano.";

                }
                return Json(new
                {
                    success = true,
                    message = "Lokalitu si převzal: " + dieslovaniTaken.Technik.Jmeno + " " + dieslovaniTaken.Technik.Prijmeni,
                    tempMessage = TempData["Zprava"]  // Return the message for the modal
                });


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Chyba při převzetí "});
            }
            
        }


        public async Task<IActionResult> GetTableDataRunningTable(int start = 0, int length = 0)
        {
            int totalRecords = _context.DieslovaniS.Include(o => o.Odstavka).Where(o => o.Odstavka.ZadanVstup == true).Count();
            length = totalRecords;
            var DieslovaniRunningList = await _context.DieslovaniS
            .Include(o=>o.Odstavka)
            .ThenInclude(o=>o.Lokality)
            .Include(t=>t.Technik)
            .Where(i=>i.Odstavka.ZadanVstup==true)
            .OrderBy(o=>o.Odstavka.Od)
            .Skip(start)
            .Take(length)
            .Select(l=> new{
                l.IdDieslovani,
                l.Odstavka.Distributor,
                l.Odstavka.Lokality.Lokalita,
                l.Odstavka.Lokality.Klasifikace,
                l.Technik.Jmeno,
                l.Technik.Prijmeni,
                l.Vstup,
                l.Odstavka.Popis,
                l.Odstavka.Lokality.Baterie,
                l.Odstavka.Lokality.Zasuvka,
                
            })
            .ToListAsync();

            return Json(new 
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(), // Unikátní ID požadavku
                recordsTotal = totalRecords, // Celkový počet záznamů
                recordsFiltered = totalRecords, // Může být upraven při vyhledávání
                data = DieslovaniRunningList // Data aktuální stránky
            });
            
        }
        public async Task<TableDieslovani> FitleredData(IQueryable<TableDieslovani> query)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userId = currentUser?.Id;
            
            
            bool isTechnik = await _userManager.IsInRoleAsync(currentUser, "Engineer");


             query = _context.DieslovaniS
            .Include(o => o.Odstavka).ThenInclude(o => o.Lokality).ThenInclude(o => o.Region)
            .Include(t => t.Technik).ThenInclude(t => t.Firma).Include(t=>t.Technik).ThenInclude(t=>t.User);

            if (isTechnik)
            {
                query = query.Where(d => d.Technik.User.Id == userId);
                Debug.WriteLine($"Filtrování: {query}");

            }

            return query.ToListAsync();
        }
        public async Task<IActionResult> GetTableDataAllTable(int start = 0, int length = 0)
        {
           
            FitleredData();  


            int totalRecords = await query.CountAsync();    
            length = totalRecords;

            var DieslovaniRunningList = await query
            .OrderBy(o => o.Odstavka.Od)
            .Skip(start)
            .Take(length)
            .Select(l=> new{
                l.IdDieslovani,
                l.Odstavka.Distributor,
                l.Odstavka.Lokality.Lokalita,
                l.Odstavka.Lokality.Klasifikace,
                l.Odstavka.Lokality.Adresa,
                l.Technik.Firma.NázevFirmy,
                l.Technik.Jmeno,
                l.Technik.Prijmeni,
                l.Odstavka.ZadanVstup,    // Zajistíme, že ZadanVstup je v datech
                l.Odstavka.ZadanOdchod,  // Zajistíme, že ZadanOdchod je v datech
                l.Technik.IdTechnika, 
                l.Odstavka.Lokality.Region.NazevRegionu,
                l.Odstavka.Od,
                l.Odstavka.Do,
                l.Vstup,
                l.Odchod,
                l.Odstavka.Popis,
                l.Odstavka.Lokality.Baterie,
                l.Odstavka.Lokality.Zasuvka,
              
            })
            .ToListAsync();

           
            return Json(new 
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(), // Unikátní ID požadavku
                recordsTotal = totalRecords, // Celkový počet záznamů
                recordsFiltered = totalRecords, // Může být upraven při vyhledávání
                data = DieslovaniRunningList // Data aktuální stránky
            });
            
        }

        public async Task<IActionResult> GetTableDatathrashTable(int start = 0, int length = 0)
        {
            int totalRecords = _context.DieslovaniS.Include(o => o.Technik).Where(o => o.Technik.IdTechnika == "606794494").Count();
            length = totalRecords;
            var DieslovaniRunningList = await _context.DieslovaniS
            .Include(o=>o.Odstavka)
            .ThenInclude(o=>o.Lokality)
            .ThenInclude(o=>o.Region)
            .Include(t=>t.Technik)
            .Where(o => o.Odstavka.ZadanOdchod==false && o.Odstavka.ZadanVstup==false && o.Technik.IdTechnika =="606794494")
            .OrderBy(o=>o.Odstavka.Od)
            .Skip(start)
            .Take(length)
            .Select(l=> new{
                l.IdDieslovani,
                l.Odstavka.Distributor,
                l.Odstavka.Lokality.Lokalita,
                l.Odstavka.Lokality.Klasifikace,
                l.Odstavka.Lokality.Region.Firma.NázevFirmy
          
            })
            .ToListAsync();

            return Json(new 
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(), // Unikátní ID požadavku
                recordsTotal = totalRecords, // Celkový počet záznamů
                recordsFiltered = totalRecords, // Může být upraven při vyhledávání
                data = DieslovaniRunningList // Data aktuální stránky
            });  
        } 



        public async Task<IActionResult> GetTableUpcomingTable(int start = 0, int length = 0)
        {
            int totalRecords = _context.DieslovaniS.Include(o => o.Odstavka).Where(o => o.Odstavka.ZadanVstup == false && o.Odstavka.ZadanOdchod == false && o.Odstavka.Od.Date==DateTime.Today &&  o.Technik.IdTechnika != "606794464").Count();
            length = totalRecords;
            var DieslovaniRunningList = await _context.DieslovaniS
            .Include(o=>o.Odstavka)
            .ThenInclude(o=>o.Lokality)
            .Include(t=>t.Technik)
            .Where(o => o.Odstavka.ZadanVstup == false && o.Odstavka.ZadanOdchod == false && o.Odstavka.Od.Date==DateTime.Today &&  o.Technik.IdTechnika != "606794494")
            .OrderBy(o=>o.Odstavka.Od)
            .Skip(start)
            .Take(length)
            .Select(l=> new{
                l.IdDieslovani,
                l.Odstavka.Distributor,
                l.Odstavka.Lokality.Lokalita,
                l.Odstavka.Lokality.Klasifikace,
                l.Technik.Jmeno,
                l.Technik.Prijmeni,
                l.Odstavka.Od.AddHours(2).Date,
                l.Odstavka.Popis,
                l.Odstavka.Lokality.Baterie,
                l.Odstavka.Lokality.Zasuvka,
            })
            .ToListAsync();

            return Json(new 
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(), 
                recordsTotal = totalRecords, 
                recordsFiltered = totalRecords, 
                data = DieslovaniRunningList 
            });
            
        }
        public async Task<IActionResult> GetTableDataEndTable(int start = 0, int length = 0)
        {
            int totalRecords = _context.DieslovaniS.Include(o => o.Odstavka).Where(o => o.Odstavka.ZadanOdchod==true && o.Odstavka.ZadanVstup==false).Count();
            length = totalRecords;
            var DieslovaniRunningList = await _context.DieslovaniS
            .Include(o=>o.Odstavka)
            .ThenInclude(o=>o.Lokality)
            .Include(t=>t.Technik)
            .Where(o => o.Odstavka.ZadanOdchod==true && o.Odstavka.ZadanVstup==false)
            .OrderBy(o=>o.Odstavka.Od)
            .Skip(start)
            .Take(length)
            .Select(l=> new{
                l.IdDieslovani,
                l.Odstavka.Distributor,
                l.Odstavka.Lokality.Lokalita,
                l.Odstavka.Lokality.Klasifikace,
                l.Odchod,
            })
            .ToListAsync();

            return Json(new 
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(), // Unikátní ID požadavku
                recordsTotal = totalRecords, // Celkový počet záznamů
                recordsFiltered = totalRecords, // Může být upraven při vyhledávání
                data = DieslovaniRunningList // Data aktuální stránky
            });
            
        }

        public async Task<IActionResult> Delete (int iDdieslovani)
        {
            
           try
            {
                var dieslovani = await _context.DieslovaniS.FindAsync(iDdieslovani);
                if (dieslovani == null)
                {
                    return Json(new { success = false, message = "Záznam nebyl nalezen." });
                }
                
    
                if (dieslovani != null)
                {
                    var technik = await _context.TechniS.Where(p => p.IdTechnika == dieslovani.IdTechnik).FirstOrDefaultAsync();
                    if (technik != null)
                    {
                        _context.DieslovaniS.Remove(dieslovani);
                        var anotherDiesel = await _context.DieslovaniS.Include(o => o.Technik).Where(o => o.Technik.IdTechnika == technik.IdTechnika).FirstOrDefaultAsync();
                        
                        if(anotherDiesel!=null)
                        {
                            technik.Taken = true;
                            _context.TechniS.Update(technik);
                        }
                        else
                        {
                            technik.Taken = false;
                            _context.TechniS.Update(technik);
                        }
                        
                    }
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Záznam byl úspěšně smazán." });

                }
                else
                {
                    return Json(new { success = false, message = "Chyba při mazání záznamu: "  });
                }
             
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Chyba při mazání záznamu: " + ex.Message });
            }
        } 

        public async Task<TableDieslovani> CreateNewDieslovaniAsync(TableOdstavky newOdstavky, TableTechnici technik)
        {   
            var NewDieslovani = new TableDieslovani
            {
                Vstup = DateTime.MinValue,
                Odchod = DateTime.MinValue,
                IDodstavky = newOdstavky.IdOdstavky,
                IdTechnik = technik.IdTechnika,
                FirmaId = newOdstavky.Lokality.Region.IdRegion
            };
            _context.DieslovaniS.Add(NewDieslovani);
            technik.Taken = true;
            _context.TechniS.Update(technik);
            await _context.SaveChangesAsync();
            Debug.WriteLine($"Dieslovani s fiktiviním technikem: {NewDieslovani.IdDieslovani}");

            return NewDieslovani;
        }
    

    }
}