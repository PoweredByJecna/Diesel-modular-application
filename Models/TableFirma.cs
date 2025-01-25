using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diesel_modular_application.Models
{
    public class TableFirma
    {
        [Key]
        public int IDFirmy{get;set;}
        public string NazevFirmy{get;set;}
        public virtual ICollection<TableDieslovani> DieslovaniList {get;set;}
        
    }
}