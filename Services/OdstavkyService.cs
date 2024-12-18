using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using Microsoft.EntityFrameworkCore;

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
        var odstavkyList = _context.OdstavkyS
        .ToList();

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

        var totalOdstavky = odstavkyList.Count();

        var regionStats = regions.Select(region => new RegionStats
        {
        RegionName = region,
        // Počet odstávek pro tento region
        Count = odstavkyList.Count(o =>
        o.Lokality.Region.NazevRegionu == region), 

        // Výpočet procenta odstávek v regionu vůči celkovým odstávkám
        Percentage = totalOdstavky == 0 ? 0 : (double)odstavkyList.Count(o =>
        o.Lokality.Region.NazevRegionu == region) / totalOdstavky * 100,

        // Nastavení barvy na základě počtu
        StatusColor = totalOdstavky == 0 ? "gray" : "green" // Změňte na podmínky pro jiné barvy podle potřeby
        }).ToList();


        return new StatsViewModel
        {
            TotalOdstavky = totalOdstavky,
            Regions = regionStats
        };
    }
}

}
