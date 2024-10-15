using System.Security.Claims;
using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> Zapis (TablePohotovosti pohotovosti)
        {
            
            var currentUser = await _userManager.GetUserAsync(User);

            if(User.IsInRole("Engineer"))
            {
                
                var Zapis = new TablePohotovosti
                {
                    IdTechnik=currentUser.Id,
                    

                };
               
            }

            return Redirect("/Odstavky/Index");
        }


    }
}