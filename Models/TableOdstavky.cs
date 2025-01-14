using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Drawing;

namespace Diesel_modular_application.Models
{
    public class TableOdstavky
    {
        [Key]
        public int IdOdstavky { get; set; }
        public string Distributor { get; set; }
        public string Firma { get; set; }
        public DateTime Od { get; set; }
        public DateTime Do { get; set; }
        public string Popis { get; set; }

        [ForeignKey("Lokalita")]
        public int LokalitaId { get; set; }
        public virtual TableLokality Lokality { get; set; }
        public virtual ICollection<TableDieslovani> DieslovaniList {get;set;}
        public bool ZadanVstup{get;set;}=false;
        public bool ZadanOdchod{get;set;} =false;
    }
}
