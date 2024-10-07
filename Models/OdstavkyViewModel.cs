using DocumentFormat.OpenXml.Drawing;

namespace Diesel_modular_application.Models
{
    public class OdstavkyViewModel
    {
        public IEnumerable<TableOdstavky> OdstavkyList { get; set; }
        public TableOdstavky AddOdstavka { get; set; } = new TableOdstavky(); 
        public IEnumerable<TableDieslovani> DieslovaniList {get;set;}
        public TableDieslovani DieslovaniMod {get;set;} = new TableDieslovani();

        public OdstavkyViewModel()
        {
            OdstavkyList = new List<TableOdstavky>(); 
            DieslovaniList= new List<TableDieslovani>();
        }
    }
}
