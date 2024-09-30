using System.ComponentModel.DataAnnotations;
namespace Diesel_modular_application.Models
{
    public class Lokality
    {

        [Key]

        public int Id { get; set; }
        [Required]
        public required string Lokalita { get; set; }
        public required string Klasifikace { get; set; }
        public required string Adresa { get; set; }
        public required string Baterie { get; set; }
        public string DA { get; set; }
        public required string Zásuvka { get; set; }

    }
}