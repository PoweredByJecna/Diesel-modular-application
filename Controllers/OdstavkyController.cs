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
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SuggestLokalita(string query)
        {
            var lokalities = await _odstavkyService.SuggestLokalitaAsync(query);
            return Json(lokalities);
        }
        [HttpGet]
        public async Task<IActionResult> DetailOdstavky(int id)
        {
            var detail = await _odstavkyService.DetailOdstavkyAsync(id);
            if (detail == null)
                return NotFound();
            return View(detail);
        }
        [HttpGet]
        public async Task<IActionResult> DetailOdstavkyJson(int id)
        {
            var detailOdstavky = await _odstavkyService.DetailOdstavkyJsonAsync(id);
            return Json(new{
                data=detailOdstavky
            });
        }
        [HttpPost]
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
        [HttpPost]
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
        [HttpPost]
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
        [HttpPost]
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
        [HttpGet]
        public async Task<IActionResult> GetTableDataOdDetail(int id)
        {
            var odstavkaList = await _odstavkyService.GetTableDataOdDetailAsync(id);

            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                data = odstavkaList
            });
        }

    }

}