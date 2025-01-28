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
using System.Threading.Tasks;

namespace Diesel_modular_application.Controllers
{
    public class RegionyController : Controller
    {
        private readonly RegionyService _regionyService;

        public RegionyController (RegionyService regionyService)
        {
            _regionyService=regionyService;
        }

        [Authorize]
        public IActionResult Index ()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRegionDataPraha()
        {
            var regionInfo= await _regionyService.GetRegionDataPrahaAsync();
            return Json(
                new{
                    data=regionInfo
                }
            );
        } 
        [HttpGet]
        public async Task<IActionResult> GetRegionDataSeverniMorava()
        {   
            var regionInfo = await _regionyService.GetRegionDataSeverniMoravaAsync();
            return Json(
                new{
                    data=regionInfo
                }
            );
        }
        [HttpGet]
        public async Task<IActionResult> GetRegionDataJizniMorava()
        {   
            var regionInfo = await _regionyService.GetRegionDataJizniMoravaAsync();
            return Json(
                new{
                    data=regionInfo
                }
            );
        }
        [HttpGet]
        public async Task<IActionResult> GetRegionDataZapadniCechy()
        {
            var regionInfo= await _regionyService.GetRegionDataZapadniCechyAsync();
            return Json(
                new{
                    data=regionInfo
                }
            );
        }
        [HttpGet]
        public async Task<IActionResult> GetRegionDataSeverniCechy()
        {
            var regionInfo= await _regionyService.GetRegionDataSeverniCechyAsync();
            return Json(
                new{
                    data=regionInfo
                }
            );
        }
        [HttpGet]
        public async Task<IActionResult> GetRegionDataJizniCechy()
        {
            var regionInfo= await _regionyService.GetRegionDataJizniCechyAsync();
            return Json(
                new{
                    data=regionInfo
                }
            );
        }        
    }
}