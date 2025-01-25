using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diesel_modular_application.Models
{
    public class TableDieslovani
    {
        [Key]
        public int IdDieslovani{get;set;}
        public DateTime Vstup {get;set;}
        public DateTime Odchod {get;set;}
        
        [ForeignKey("Odstavka")]
        public int IDodstavky {get;set;}
        public virtual TableOdstavky Odstavka {get;set;}
        
        [ForeignKey("Technik")]
        public string IdTechnik {get;set;}
        public virtual TableTechnici Technik {get;set;}
     
    }
}