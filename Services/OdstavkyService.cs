using Diesel_modular_application.Data;
using Diesel_modular_application.Models;

namespace Diesel_modular_application.Services
{
    public class OdstavkyService
    {
        private readonly DAdatabase _context;

        public OdstavkyService(DAdatabase context)
        {
            _context = context;
        }

        public StatsViewModel GetRegionStats()
        {
            var odstavkyList = _context.OdstavkyS.ToList();
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
