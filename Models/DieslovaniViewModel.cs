namespace Diesel_modular_application.Models
{
    public class DieslovaniViewModel
    {
        public IEnumerable<TableDieslovani> DieslovaniList {get;set;}
        public TableDieslovani DislovaniMod {get;set;} = new TableDieslovani();

        public DieslovaniViewModel()
        {
            DieslovaniList= new List<TableDieslovani>();
        }
    }
}