namespace Diesel_modular_application.Models
{
    public class OdstavkyViewModel
    {
        public IEnumerable<OdstavkyTable> OdstavkyList { get; set; }
        public OdstavkyTable AddOdstavka { get; set; } = new OdstavkyTable(); 

        public OdstavkyViewModel()
        {
            OdstavkyList = new List<OdstavkyTable>(); 
        }
    }
}
