using System.Diagnostics;
using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diesel_modular_application.Controllers
{
    public class DieslovaniController:Controller
    {
        private readonly DAdatabase _context;
        private readonly UserManager<IdentityUser> _userManager;

        

        public DieslovaniController(DAdatabase context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        public async Task<IActionResult> Vstup (int IdDieslovani)
        {
            try
            {
                var dis= await _context.DieslovaniS.FindAsync(IdDieslovani);
                if(dis !=null)
                {    
                        
                        dis.Vstup=DateTime.Now;
                        _context.Update(dis);
                        await _context.SaveChangesAsync();
                        ViewBag.Message="vstup";
                }
                var odstavka = await _context.OdstavkyS.FindAsync(dis.IDodstavky);
                if (odstavka != null)
                {
                    // Nastav ZadanVstup na true
                    odstavka.ZadanVstup = true;
                    _context.Update(odstavka);
                    
                }    
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Byl zadán vstup na lokalitu." });
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
                .Include(d => d.Technik)  // Zajišťuje načtení spojeného technika
                .FirstAsync(d=>d.IdDieslovani==IdDieslovani);

                if(dis !=null)
                {    
                    dis.Technik.Taken=false;
                    dis.Odchod=DateTime.Now;
                    _context.Update(dis);
                }
                var odstavka = await _context.OdstavkyS.FindAsync(dis.IDodstavky);
                if (odstavka != null)
                {
                    // Nastav ZadanVstup na true
                    odstavka.ZadanOdchod=true;
                    odstavka.ZadanVstup=false;
                
                    _context.Update(odstavka);
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
            return Redirect ("/Home/Index");
        }
        public async Task<IActionResult> Take(int id)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var dieslovaniTaken = await _context.DieslovaniS
                .FirstAsync(d=>d.IdDieslovani==id);
                if(currentUser!=null)
                {
                    dieslovaniTaken.Technik.IdTechnika=currentUser.Id;
                    _context.Update(dieslovaniTaken);
                    return Json(new { success = true, message = "Dielolovaní si převzal." + dieslovaniTaken.Technik.Jmeno });

                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Chyba při převzetí " + ex.Message });
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
                l.Vstup,
                l.Odstavka.Popis,
                l.Odstavka.Lokality.Baterie,
                l.Odstavka.Lokality.Zásuvka,
                
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
        public async Task<IActionResult> GetTableDataAllTable(int start = 0, int length = 0)
        {
            int totalRecords = _context.DieslovaniS.Include(o => o.Odstavka).ThenInclude(o=>o.Lokality).ThenInclude(o=>o.Region).Include(t=>t.Technik).ThenInclude(t=>t.Firma).Count();
            length = totalRecords;
            var DieslovaniRunningList = await _context.DieslovaniS
            .Include(o => o.Odstavka).ThenInclude(o=>o.Lokality).ThenInclude(o=>o.Region).Include(t=>t.Technik).ThenInclude(t=>t.Firma)
            .OrderBy(o=>o.Odstavka.Od)
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
                l.Odstavka.Lokality.Zásuvka,
              
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
                l.Odstavka.Od.AddHours(2).Date,
                l.Odstavka.Popis,
                l.Odstavka.Lokality.Baterie,
                l.Odstavka.Lokality.Zásuvka,
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
                        technik.Taken = false;
                        _context.TechniS.Update(technik);
                    }
                    _context.DieslovaniS.Remove(dieslovani);
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
            await _context.SaveChangesAsync();
            technik.Taken = true;
            _context.TechniS.Update(technik);
            await _context.SaveChangesAsync();
            Debug.WriteLine($"Dieslovani s fiktiviním technikem: {NewDieslovani.IdDieslovani}");

            return NewDieslovani;
        }
    

    }
}