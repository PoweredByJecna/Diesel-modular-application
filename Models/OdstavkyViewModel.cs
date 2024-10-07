using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office.CustomUI;

namespace Diesel_modular_application.Models
{
    public class OdstavkyViewModel
    {
        public IEnumerable<TableOdstavky> OdstavkyList { get; set; }
        public TableOdstavky AddOdstavka { get; set; } = new TableOdstavky(); 
        public IEnumerable<TableDieslovani> DieslovaniList {get;set;}
        public IEnumerable<TableFirma> FirmaList{get;set;}
        public IEnumerable<TableTechnik> TechnikList{get;set;}
        public TableDieslovani DieslovaniMod {get;set;} = new TableDieslovani();
        public TableFirma FirmaMod{get;set;}=new TableFirma();
        public TableTechnik TechnikMod{get;set;}=new TableTechnik();

        public OdstavkyViewModel()
        {
            OdstavkyList = new List<TableOdstavky>(); 
            DieslovaniList= new List<TableDieslovani>();
            FirmaList= new List<TableFirma>();
            TechnikList= new List<TableTechnik>();
        }
    }
}
