using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;


namespace Diesel_modular_application.Controllers
{
    public class HomeController : Controller
    {
        private readonly DAdatabase _context;
        

        private readonly UserManager<IdentityUser> _userManager;


            private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DAdatabase context,UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        
        [Authorize]
        public async Task<IActionResult> IndexAsync(OdstavkyViewModel odstavky)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userId = currentUser?.Id; // 

            
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
