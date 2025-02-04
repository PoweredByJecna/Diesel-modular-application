using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Diesel_modular_application.Services;


namespace Diesel_modular_application.Controllers
{
    public class RegionyController(RegionyService regionyService) : Controller
    {
        private readonly RegionyService _regionyService = regionyService;

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