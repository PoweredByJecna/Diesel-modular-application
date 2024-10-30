using System.Security.AccessControl;
using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> IndexAsync(OdstavkyViewModel odstavky)
        {
            odstavky.OdstavkyList = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .ToListAsync();
            odstavky.PohotovostList = await _context.Pohotovts
                .Include(o => o.Technik)
                .ToListAsync();
            odstavky.PohotovostList = await _context.Pohotovts
                .Include(O=>O.User)
                .ToListAsync();
            odstavky.TechnikList= await _context.TechniS
                .Include(o=>o.Firma)
                .ToListAsync();
            odstavky.RegionyList=await _context.ReginoS
                .Include(O=>O.Firma)
                .ToListAsync();
      
    
                
            
            return View("Index", odstavky);
        }
        public async Task<IActionResult> Create(OdstavkyViewModel odstavky)
        {
            var lokalitaSearch = await _context.LokalityS
                .Include(l => l.Region)
                .ThenInclude(r => r.Firma)
                .FirstOrDefaultAsync(input => input.Lokalita == odstavky.AddOdstavka.Lokality.Lokalita);

            if (lokalitaSearch == null)
            {
                ViewBag.Message = "Zadaná lokalita neexistuje";
                return View("Index", odstavky);
            }
            var distrib="";
         
            if(lokalitaSearch.Adresa=="Česká Lípa")
            {
                   distrib= "ČEZ";
            }
            distrib="EGD";

            var newOdstavka = new TableOdstavky
            {            
                Distributor = distrib,
                Firma=lokalitaSearch.Region.Firma.NázevFirmy,
                Od = odstavky.AddOdstavka.Od,
                Do = odstavky.AddOdstavka.Do,
                Vstup=odstavky.AddOdstavka.Vstup,
                Odchod=odstavky.AddOdstavka.Odchod,
                Popis = odstavky.AddOdstavka.Popis,
                LokalitaId = lokalitaSearch.Id // Odkaz na ID existující lokality

            };
            if(newOdstavka.Od<DateTime.Today && newOdstavka.Od>newOdstavka.Do)
            {
                ViewBag.Message ="Špatné datum";
            }
            if(newOdstavka.Od>=DateTime.Today && newOdstavka.Od<newOdstavka.Do)
            {
            _context.OdstavkyS.Add(newOdstavka);
            await _context.SaveChangesAsync();
            ViewBag.Message = "Odstávka byla vytvořena";
            }
            var regionId = lokalitaSearch.Region.IdRegion;

            var firmaVRegionu = await _context.ReginoS
            .Where(r => r.IdRegion == regionId)
            .Select(r => r.Firma)
            .FirstOrDefaultAsync();

                   
            var TechnikSearch = await _context.Pohotovts
                .Where(p=>p.Technik.FirmaId==firmaVRegionu.IDFirmy)
                .Select(p=>p.Technik.IdTechnika)
                .FirstOrDefaultAsync();
            

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
            // Načti seznam odstávek pro zobrazení
            odstavky.DieslovaniList=await _context.DieslovaniS.ToListAsync();
            odstavky.OdstavkyList = await _context.OdstavkyS.ToListAsync();
            return Redirect("/Odstavky/Index");
        }
        public async Task<IActionResult> Vstup(OdstavkyViewModel odstavky)
        { 
            var SetOdstavka=new TableOdstavky
            {   
                Vstup=odstavky.AddOdstavka.Vstup

            };
            _context.OdstavkyS.Add(SetOdstavka);
            await _context.SaveChangesAsync();
            return Redirect("/Home/Index");

        }
        public async Task<IActionResult> Odchod(OdstavkyViewModel odstavky)
        {
            var SetOdstavka = new TableOdstavky
            {
                Odchod=odstavky.AddOdstavka.Odchod
            };
            _context.OdstavkyS.Add(SetOdstavka);
            await _context.SaveChangesAsync();
            return Redirect("/Home/Index");
        }


    }

}