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
           

                var lokalitaSearch = await _context.LokalityS.FirstOrDefaultAsync(Input => Input.Lokalita==odstavky.AddOstavka.Lokalita);
                if(lokalitaSearch==null)
                {
                    ViewBag.Message="Zadaná lokalita neexistuje";
                  
                }
                else
                {
                    ViewBag.Message="Lokalita nalezena";
                    /*                     
                    var NewOdstavka = new OdstavkyViewModel
                    {
                        IdOdstavky=odstavky.Id,
                        LokalitaInfo=lokalitaSearch.Id,
                        Od=odstavky.AddOstavka.Od,
                        Do=odstavky.AddOstavka.Do,
                        Popis=odstavky.AddOstavka.Popis,
                    };
                    _context.Add(NewOdstavka);
                    await _context.SaveChangesAsync();
                    */
                    ViewBag.Message="Odstávka byla vytvořena";
                    
                }
            //}
                return View("Index", odstavky);
        }
        
        
    }

}