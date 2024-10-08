using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Diesel_modular_application.Controllers
{
    public class PohotovostiController:Controller
    {
        private readonly DAdatabase _context;

        public PohotovostiController(DAdatabase context)
        {
            _context = context;
        }

        public async Task<IActionResult> Zapis (TablePohotovosti pohotovosti)
        {
            


            return Redirect("/Odstavky/Index");
        }


    }
}