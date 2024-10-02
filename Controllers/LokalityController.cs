
using Diesel_modular_application.Models;
using Diesel_modular_application.Data;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diesel_modular_application.Controllers
{
    public class LokalityController : Controller
    {
        private readonly DAdatabase _context;

        public LokalityController(DAdatabase context)
        {
            _context= context;
        }

        [Authorize]
           public IActionResult Index()
        {
            var lokality = _context.LokalityS.ToList();

            return View(lokality);
        }
       // [Authorize(Roles ="Admin")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if(file != null && file.Length>0)
            {
                ExcelHandling excelFileHandling = new ExcelHandling();
                var lokalityS = excelFileHandling.ParseExcelFile(file.OpenReadStream());

                await _context.LokalityS.AddRangeAsync(lokalityS);
                await _context.SaveChangesAsync();

                return View("Index");


            }
            return View();
        }
    

    }
}