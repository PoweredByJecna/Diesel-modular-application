namespace Diesel_modular_application.Models
{
    public class OdstavkyViewModel
    {
        public IEnumerable<TableOdstavky> OdstavkyList { get; set; }
        public TableOdstavky AddOdstavka { get; set; } = new TableOdstavky(); 

        public OdstavkyViewModel()
        {
            OdstavkyList = new List<TableOdstavky>(); 
        }
    }
}
