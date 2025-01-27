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
            var regionyInfoPraha= await _regionyService.GetRegionDataPrahaAsync();
            return Json(
                new{
                    data=regionyInfoPraha
                }
            );
        }   
    }
}