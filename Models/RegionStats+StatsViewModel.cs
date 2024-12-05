namespace Diesel_modular_application.Models
{

        public class RegionStats
    {
        public string RegionName { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
    public class StatsViewModel
    {
        public int TotalOdstavky { get; set; }
        public List<RegionStats> Regions { get; set; }
    }
}
