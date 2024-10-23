using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Bibliography;
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
            return View("Index", odstavky);
        }
        public async Task<IActionResult> Create(OdstavkyViewModel odstavky)
        {
            var lokalitaSearch = await _context.LokalityS.FirstOrDefaultAsync(input => input.Lokalita == odstavky.AddOdstavka.Lokality.Lokalita);
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
                Firma="VEGACOM",
                Od = odstavky.AddOdstavka.Od,
                Do = odstavky.AddOdstavka.Do,
                Vstup=odstavky.AddOdstavka.Vstup,
                Odchod=odstavky.AddOdstavka.Odchod,
                Popis = odstavky.AddOdstavka.Popis,
                LokalitaId = lokalitaSearch.Id // Odkaz na ID existující lokality
            };

            _context.OdstavkyS.Add(newOdstavka);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Odstávka byla vytvořena";

            // Načti seznam odstávek pro zobrazení
            odstavky.OdstavkyList = await _context.OdstavkyS.ToListAsync();
            return View("Index", odstavky);
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