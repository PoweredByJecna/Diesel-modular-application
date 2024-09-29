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
        public async Task<IActionResult> Create(Odstavky odstavky)
        {   
            if(ModelState.IsValid)
            {   
                var NewOdstavka = await _context.LokalityS.FirstOrDefaultAsync(Input => Input.Lokalita==odstavky.Lokalita);
                if(NewOdstavka==null)
                {
                    ModelState.AddModelError(string.Empty, "Zadana lokalita neexistuje");
                    return View();
                }
             
                   NewOdstavka.Klasifikace=odstavky.Klasifikace; 
                   NewOdstavka.Adresa=odstavky.Adresa;
                   NewOdstavka.Baterie=odstavky.Baterie;
                   NewOdstavka.Zásuvka=odstavky.Zásuvka;
                


            }
           return View();
        }
    }

}