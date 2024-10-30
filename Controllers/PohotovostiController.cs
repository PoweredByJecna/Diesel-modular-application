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
            
            [Authorize(Roles="Engineer")]
            public async Task<IActionResult> Zapis (OdstavkyViewModel pohotovosti)
            {
                
                var currentUser = await _userManager.GetUserAsync(User);

                if(User.IsInRole("Engineer"))
                {
                    var technikSearch = await _context.TechniS.FirstOrDefaultAsync(input=>input.IdUser==currentUser.Id);
                    if(technikSearch!=null)
                    {
                    ViewBag.Message=technikSearch.Jmeno;

                    var Zapis = new TablePohotovosti
                    {
                    IdUser=currentUser.Id,  
                    Začátek=pohotovosti.PohotovostMod.Začátek,
                    Konec=pohotovosti.PohotovostMod.Konec,
                    IdTechnik=technikSearch.IdTechnika 
                    };
                    
                    _context.Pohotovts.Add(Zapis);
                    await _context.SaveChangesAsync();
                    
                    }

                    if (technikSearch == null)
                    {
                        ViewBag.Message = "Zadaná lokalita neexistuje";
                    return Redirect("/Odstavky/Index");
                    }
                
                }

            pohotovosti.PohotovostList = await _context.Pohotovts.ToListAsync();
                return View("Index", pohotovosti);
            }


        }
    }