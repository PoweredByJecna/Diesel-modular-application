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

    public class OdstavkyServiceController : Controller
    {
        private readonly DAdatabase _context;
        
        public OdstavkyServiceController( DAdatabase context)
        {
            
            _context = context;
           
        }

        public StatsViewModel GetRegionStats()
        {
            var odstavkyList = _context.OdstavkyS.ToList(); // Nahraďte skutečným zdrojem dat
            var today = DateTime.Today;

            var regions = new List<string>
            {
                "Západní Čechy",
                "Jižní Čechy",
                "Praha + Střední Čechy",
                "Severní Morava",
                "Jižní Morava",
                "Severní Čechy"
            };

            var totalOdstavky = odstavkyList.Count(d =>
                d.Od.Date == today &&
                (d.ZadanVstup || d.Od.Date == today) &&
                d.Lokality.DA == "FALSE");

            var regionStats = regions.Select(region => new RegionStats
            {
                RegionName = region,
                Count = odstavkyList.Count(o =>
                    o.Lokality.Region.NazevRegionu == region &&
                    o.Od.Date == today &&
                    (o.ZadanVstup || o.Od.Date == today)),
                Percentage = totalOdstavky == 0 ? 0 : (double)odstavkyList.Count(o =>
                    o.Lokality.Region.NazevRegionu == region &&
                    o.Od.Date == today &&
                    (o.ZadanVstup || o.Od.Date == today)) / totalOdstavky * 100
            }).ToList();

            return new StatsViewModel
            {
                TotalOdstavky = totalOdstavky,
                Regions = regionStats
            };
        }
    }


}