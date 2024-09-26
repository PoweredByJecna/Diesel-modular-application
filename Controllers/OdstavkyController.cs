using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc;

namespace Diesel_modular_application.Controllers
{
    public class OdstavkyController:Controller
    {
        private readonly DAdatabase _context;

        public OdstavkyController(DAdatabase context)
        {
            _context= context;
        }
        public IActionResult Index()
        {
                return View();
        }
        public async Task<IActionResult> Create(Odstavky odstavky)
        {   
            var NewOdstavka = _context.LokalityS;
            
            if(ModelState.IsValid)
            {
                
            }

            return View();
        }
    }

}