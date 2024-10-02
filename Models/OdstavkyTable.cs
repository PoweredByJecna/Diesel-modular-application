using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Diesel_modular_application.Models
{
    public class OdstavkyTable
    {
        [Key]
        public  int IdOdstavky { get; set; }
        public  string Distributor { get; set; }
        public  DateTime Od { get; set; }
        public  DateTime Do { get; set; }
        public string Popis { get; set; }

        [ForeignKey("Lokalita")]
        public  int LokalitaId { get; set; }
        public virtual LokalityTable Lokality { get; set; }
    }
}
