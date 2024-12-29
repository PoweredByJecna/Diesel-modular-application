    using System.Security.Claims;
    using Diesel_modular_application.Data;
    using Diesel_modular_application.Models;
    using DocumentFormat.OpenXml.Spreadsheet;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace Diesel_modular_application.Controllers
    {
        public class PohotovostiController:Controller
        {
            private readonly UserManager<IdentityUser> _userManager;
            private readonly DAdatabase _context;
            
            public PohotovostiController(DAdatabase context,UserManager<IdentityUser> userManager)
            {
                _context = context;
                _userManager=userManager;
            }
            public async Task<IActionResult> IndexAsync(OdstavkyViewModel pohotovosti)
            {
                pohotovosti.PohotovostList = await _context.Pohotovts
                    .Include(o => o.Technik)
                    .ToListAsync();
                return View("Index", pohotovosti);
            }
            
            [Authorize(Roles="Engineer, Admin")]
            public async Task<IActionResult> Zapis (OdstavkyViewModel pohotovosti)
            {
                
                var currentUser = await _userManager.GetUserAsync(User);

                if(User.IsInRole("Engineer"))
                {
                    var technikSearch = await _context.TechniS.FirstOrDefaultAsync(input=>input.IdUser==currentUser.Id);

                    if(technikSearch!=null)
                    {
                       
                        ViewBag.Message=technikSearch.Jmeno;
                        if(pohotovosti.PohotovostMod.Konec>pohotovosti.PohotovostMod.Začátek && pohotovosti.PohotovostMod.Začátek>=DateTime.Today)
                        {
                        var Zapis = new TablePohotovosti
                        {
                            IdUser=technikSearch.IdUser,  
                            Začátek=pohotovosti.PohotovostMod.Začátek,
                            Konec=pohotovosti.PohotovostMod.Konec,
                            IdTechnik=technikSearch.IdTechnika 
                        };
                        
                        _context.Pohotovts.Add(Zapis);
                        await _context.SaveChangesAsync();
                        }
                    }

                    if (technikSearch == null)
                    {
                        ViewBag.Message = "Zadaná lokalita neexistuje";
                        return Redirect("/Odstavky/Index");
                    }

                }
                if(User.IsInRole("Admin"))
                {
                    var technikSearch = await _context.TechniS.FirstOrDefaultAsync(input=>input.IdTechnika==pohotovosti.TechnikMod.IdTechnika);
                    if(pohotovosti.PohotovostMod.Konec>pohotovosti.PohotovostMod.Začátek && pohotovosti.PohotovostMod.Začátek>=DateTime.Today)
                    {
                    var Zapis = new TablePohotovosti
                    {
                        IdUser=technikSearch.IdUser,  
                        Začátek=pohotovosti.PohotovostMod.Začátek,
                        Konec=pohotovosti.PohotovostMod.Konec,
                        IdTechnik=technikSearch.IdTechnika 
                    };
                        
                    _context.Pohotovts.Add(Zapis);
                    await _context.SaveChangesAsync();
                }

                
            }
            pohotovosti.PohotovostList = await _context.Pohotovts.ToListAsync();
            return Redirect("/Odstavky/Index");


        }
        public async Task<IActionResult> GetTableDatapohotovostiTable(int start = 0, int length = 0)
        {
            int totalRecords = await _context.Pohotovts.CountAsync();
            length = totalRecords;

            var pohotovostTechnikIds = await _context.Pohotovts
            .Select(p => p.Technik.IdTechnika)
            .Distinct()
            .ToListAsync();

            // Vytvoření mapy techniků a jejich lokalit (TechnikLokalitaMap) pouze pro techniky v pohotovosti
            var technikLokalitaMap = await _context.DieslovaniS
            .Include(o => o.Odstavka)
            .ThenInclude(l => l.Lokality)
            .Where(d => pohotovostTechnikIds.Contains(d.IdTechnik)) // Pouze technici, kteří jsou v pohotovosti a na dieslování
            .GroupBy(d => d.IdTechnik)
            .ToDictionaryAsync(
            group => group.Key,
            group => group
            .OrderBy(o=>o.Odstavka.Od)
            .Select(d => d.Odstavka.Lokality.Lokalita).FirstOrDefault()
            );

   
            var pohotovostList = await _context.Pohotovts
            .Include(o => o.Technik)
            .ThenInclude(o => o.User)
            .Include(o => o.Technik)
            .ThenInclude(o => o.Firma)
            .OrderBy(o => o.Začátek)
            .Skip(start)
            .Take(length)
            .Select(l => new
            {
                l.Technik.Jmeno,
                PhoneNumber = l.Technik.User.PhoneNumber,
                Firma = l.Technik.Firma.NázevFirmy,
                l.Začátek,
                l.Konec,
                l.Technik.Taken,
                Lokalita = technikLokalitaMap.ContainsKey(l.Technik.IdTechnika)
                    ? technikLokalitaMap[l.Technik.IdTechnika] 
                    : "Nemá přiřazenou lokalitu" 
            })
            .ToListAsync();

        return Json(new
        {
            draw = HttpContext.Request.Query["draw"].FirstOrDefault(), 
            recordsTotal = totalRecords, 
            recordsFiltered = totalRecords, 
            data = pohotovostList 
        });
        }

    }
}