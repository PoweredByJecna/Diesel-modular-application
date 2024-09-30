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
        public async Task<IActionResult>Create(OdstavkyViewModel odstavky)
        {   
            //if(ModelState.IsValid)
           

                var NewOdstavka = await _context.LokalityS.FirstOrDefaultAsync(Input => Input.Lokalita==odstavky.NewOdstavka.Lokalita);
                if(NewOdstavka==null)
                {
                    ViewBag.Message="Zadaná lokalita neexistuje";
                  
                }
                else
                {
                    ViewBag.Message="Lokalita nalezena";
                    odstavky.NewOdstavka.Distributor=odstavky.NewOdstavka.Distributor;
                    odstavky.NewOdstavka.Klasifikace=NewOdstavka.Klasifikace;
                    odstavky.NewOdstavka.Adresa=NewOdstavka.Adresa;
                    odstavky.NewOdstavka.Od=odstavky.NewOdstavka.Od;
                    odstavky.NewOdstavka.Do=odstavky.NewOdstavka.Do;
                    odstavky.NewOdstavka.Baterie=NewOdstavka.Baterie;
                    odstavky.NewOdstavka.Baterie=NewOdstavka.Zásuvka;
                    odstavky.NewOdstavka.Popis=odstavky.NewOdstavka.Popis;

                    _context.Add(odstavky);
                    await _context.SaveChangesAsync();
                    ViewBag.Message="Odstávka byla vytvořena";
                    
                }
            //}
                return View("Index", odstavky);
        }
        
        
    }

}