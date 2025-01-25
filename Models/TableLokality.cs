using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diesel_modular_application.Models
{
    public class TableLokality
    {

        [Key]
        public int Id { get; set; }
        public string Lokalita { get; set; }
        public string Klasifikace { get; set; }
        public string Adresa { get; set; }
        public string Baterie { get; set; }
        public bool DA { get; set; }
        public bool Zasuvka { get; set; }

        [ForeignKey("Region")]
        public int? RegionID {get;set;}
        public virtual TableRegiony Region {get;set;}

        public virtual ICollection<TableOdstavky> OdstavkyList { get; set; }
      
    }
}
