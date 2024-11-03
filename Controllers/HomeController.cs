using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Diesel_modular_application.Controllers
{
    public class HomeController : Controller
    {
        private readonly DAdatabase _context;

       

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DAdatabase context)
        {
            _logger = logger;
            _context = context;
        }

        
        [Authorize]
        public async Task<IActionResult> IndexAsync(OdstavkyViewModel odstavky)
        {
            odstavky.OdstavkyList = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .ThenInclude(l => l.Region)
                .ThenInclude(l=>l.Firma)
                .ToListAsync();
            odstavky.DieslovaniList =await _context.DieslovaniS
                .Include(o=>o.Technik)
                .ToListAsync();
            odstavky.LokalityList=await _context.LokalityS.ToListAsync();
            return View("Index", odstavky);
        }
        public async Task<IActionResult> Test (OdstavkyViewModel odstavky )
        {
            
            var distrib="";
            
            for(int i=1;i<=10;i++)
            {
                if(odstavky.LokalityList.Any())
                {
                var IdNumber =RandomNumberGenerator.GetInt32(1,odstavky.LokalityList.Count());
                var lokalitaSearch = await _context.LokalityS.Where(o=>o.Id==IdNumber).FirstOrDefaultAsync();
                    if(lokalitaSearch.Region.NazevRegionu=="Severní Čechy" || lokalitaSearch.Region.NazevRegionu=="Západní Čechy" || lokalitaSearch.Region.NazevRegionu=="Severní Morava")
                    {
                        distrib= "ČEZ";
                    }
                    if(lokalitaSearch.Region.NazevRegionu=="Jižní Morava" || lokalitaSearch.Region.NazevRegionu=="Jižní Čechy")
                    {
                        distrib= "EGD";
                    }
                    if(lokalitaSearch.Region.NazevRegionu=="Praha + Střední Čechy")
                    {
                        distrib= "PRE";
                    }
                    var newOdstavka = new TableOdstavky
                    {            
                        Distributor = distrib,
                        Firma=lokalitaSearch.Region.Firma.NázevFirmy,
                        Od = DateTime.Today,
                        Do = DateTime.Today.AddHours(8),
                        Vstup=odstavky.AddOdstavka.Vstup,
                        Odchod=odstavky.AddOdstavka.Odchod,
                        Popis = "Test",
                        LokalitaId = lokalitaSearch.Id 
                    };
                    var regionId = lokalitaSearch.Region.IdRegion;

                var firmaVRegionu = await _context.ReginoS
                .Where(r => r.IdRegion == regionId)
                .Select(r => r.Firma)
                .FirstOrDefaultAsync();

                
                var TechnikSearch = await _context.Pohotovts
                .Where(p=>p.Technik.FirmaId==firmaVRegionu.IDFirmy)
                .Select(p=>p.Technik.IdTechnika)
                .FirstOrDefaultAsync();

                if (TechnikSearch==null)
                {
                    return Redirect ("/Odstavky/Index");
                }
            
                if(TechnikSearch!=null)
                {
                    var NewDieslovani = new TableDieslovani
                    {
                        Vstup=odstavky.DieslovaniMod.Vstup,
                        Odchod=odstavky.DieslovaniMod.Odchod,
                        IDodstavky=newOdstavka.IdOdstavky,
                        IdTechnik=TechnikSearch,
                        FirmaId=firmaVRegionu.IDFirmy
                    };
                    _context.DieslovaniS.Add(NewDieslovani);
                    await _context.SaveChangesAsync();
                    var technik = await _context.TechniS.FindAsync(TechnikSearch);
                    
                    if(odstavky.AddOdstavka.Od.Date==DateTime.Today)
                    {
                        technik.Taken=true;
                        _context.TechniS.Update(technik);
                        TempData["Zprava"] = "TechnikUpdate";
                    }
                    await _context.SaveChangesAsync();
                }
                }

            }
            await _context.SaveChangesAsync();
            return Redirect("/Home/Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
