using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Diesel_modular_application.Models
{
    public class Odstavky:Lokality
    {
        public required int IdOdstavky{get;set;}
        public required string Distributor {get; set;}
        public required DateTime Od {get; set;}
        public required DateTime Do {get; set;}
        public string Popis{get; set;}

        [ForeignKey("Lokalita")]
        public required int LokalitaInfo { get; set; }
        public virtual Lokality Lokality { get; set;}
    }       
}       