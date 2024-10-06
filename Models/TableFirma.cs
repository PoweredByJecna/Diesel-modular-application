using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diesel_modular_application.Models
{
    public class TableFirma
    {
        [Key]
        public int IDFirmy{get;set;}
        public string NázevFirmy{get;set;}

        [ForeignKey("Region")]
        public int RegionId {get;set;}
        public virtual TableRegiony Regiony {get;set;}
    }
}