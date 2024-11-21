using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
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
        public async Task<IActionResult> GetTableData(string tableId, int page = 1)
        {
        int pageSize = 4;
        if (tableId == "upcoming")
        {
        var odstavkyQuery = _context.DieslovaniS
            .Include(o => o.Odstavka)
            .ThenInclude(o => o.Lokality)
            .Where(d => d.Odstavka.Od.Date == DateTime.Today)
            .OrderBy(o => o.Odstavka.Od);

        var dieslovaniList = await odstavkyQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(await odstavkyQuery.CountAsync() / (double)pageSize);

        return Json(new { Data = dieslovaniList, CurrentPage = page, TotalPages = totalPages });
        }

        if (tableId == "all")
        {
        var dieslovaniAll = _context.DieslovaniS
            .Include(o => o.Odstavka)
            .ThenInclude(o => o.Lokality)
            .Where(d => d.Odstavka.Od.Date == DateTime.Today)
            .OrderBy(o => o.Odstavka.Od);

        var dieslovaniList = await dieslovaniAll
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(await dieslovaniAll.CountAsync() / (double)pageSize);

        return Json(new { Data = dieslovaniList, CurrentPage = page, TotalPages = totalPages });
        }

        return BadRequest("Invalid tableId");
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
