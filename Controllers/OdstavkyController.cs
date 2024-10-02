using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diesel_modular_application.Controllers
{
    public class OdstavkyController:Controller
    {
        private readonly DAdatabase _context;

        public OdstavkyController(DAdatabase context)
        {
            _context= context;
        }
        
        [Authorize]
        public IActionResult Index()
        {
         
            return View();
        }
        public async Task<IActionResult> Create(OdstavkyViewModel odstavky)
        {
            var lokalitaSearch = await _context.LokalityS.FirstOrDefaultAsync(input => input.Lokalita == odstavky.AddOdstavka.Lokality.Lokalita);
            if (lokalitaSearch == null)
            {
                ViewBag.Message = "Zadaná lokalita neexistuje";
                return View("Index", odstavky);
            }

            var newOdstavka = new OdstavkyTable
            {
                Distributor = odstavky.AddOdstavka.Distributor,
                Od = odstavky.AddOdstavka.Od,
                Do = odstavky.AddOdstavka.Do,
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


    }

}