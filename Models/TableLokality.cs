using System.ComponentModel.DataAnnotations;

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
        public string DA { get; set; }
        public string Zásuvka { get; set; }

        public virtual ICollection<TableOdstavky> OdstavkyList { get; set; }
    }
}
