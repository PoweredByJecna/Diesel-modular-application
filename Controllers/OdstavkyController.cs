using System.Security.AccessControl;
using System.Security.Cryptography;
using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Diesel_modular_application.KlasifikaceRule;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Diesel_modular_application.Services;
using System.Diagnostics;
using AspNetCoreGeneratedDocument;
using Humanizer;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace Diesel_modular_application.Controllers
{
    public class OdstavkyController : Controller
    {
        private readonly DAdatabase _context;
        private readonly OdstavkyService _odstavkyService;



        public OdstavkyController(DAdatabase context, OdstavkyService odstavkyService)
        {
            _context = context;
            _odstavkyService = odstavkyService;
        }

        [Authorize]
        public async Task<IActionResult> IndexAsync(OdstavkyViewModel odstavky)
        {


            odstavky.OdstavkyList = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .ToListAsync();
            odstavky.PohotovostList = await _context.Pohotovts
                .Include(o => o.Technik)
                .ThenInclude(o=>o.User)
                .ToListAsync();
            odstavky.TechnikList = await _context.TechniS
                .Include(o => o.Firma)
                .ToListAsync();
            odstavky.RegionyList = await _context.ReginoS
                .Include(O => O.Firma)
                .ToListAsync();
            odstavky.DieslovaniList = await _context.DieslovaniS
                .Include(o => o.Technik)
                .ToListAsync();

            var odstavkyQuery = _context.OdstavkyS
                .Include(o => o.Lokality)
                .OrderBy(o => o.IdOdstavky);

    

            odstavky.RegionStats = _odstavkyService.GetRegionStats();


            var id = await _context.DieslovaniS
            .Include(o => o.Technik)
            .Where(static o => o.Technik.Taken == true)
            .Select(o => o.IdTechnik)
            .ToListAsync();
            
            return View("Index", odstavky);
        }

        public async Task<IActionResult> SuggestLokalita(string query)
        {
            var lokalities = await _odstavkyService.SuggestLokalitaAsync(query);
            return Json(lokalities);
        }
 
        public async Task<IActionResult> Create(string lokalita, DateTime od, DateTime DO, string popis)
        {
            var result = await _odstavkyService.CreateOdstavkaAsync(lokalita, od, DO, popis);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            else
            {
                return Json(new
                {
                    success = true,
                    message = result.Message,
                    odstavkaId = result.Odstavka?.IdOdstavky,
                    dieslovaniId = result.Dieslovani?.IdDieslovani
                });
            }
        }
        public async Task<IActionResult> Test()
        {
            var result = await _odstavkyService.TestOdstavkaAsync();
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            else
            {
                return Json(new
                {
                    success = true,
                    message = result.Message,
                    odstavkaId = result.Odstavka?.IdOdstavky,
                    dieslovaniId = result.Dieslovani?.IdDieslovani
                });
            }
        }

        public async Task<IActionResult> Delete(int idodstavky)
        {
            var result = await _odstavkyService.DeleteOdstavkaAsync(idodstavky);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            else
            {
                return Json(new { success = true, message = result.Message });
            }
        }

        public async Task<IActionResult> GetTableData(int start = 0, int length = 0)
        {
            var (totalRecords, data) = await _odstavkyService.GetTableDataAsync(start, length);

            // Vracíme data ve formátu, který DataTables očekává
            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }

        public async Task<IActionResult> GetTableDataOdDetail(int id)
        {
            var odstavkaList = await _odstavkyService.GetTableDataOdDetailAsync(id);

            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                data = odstavkaList
            });
        }

        public IActionResult GetOdstavkyData()
        {
            var stats = _odstavkyService.GetRegionStats();
            return Ok(stats);
        }




    }

}