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
        [ForeignKey("Firma")]
        public int FirmaId{get;set;}
        public virtual TableFirma Firma {get;set;}
         [ForeignKey("Technik")]
        public int IdTechnik{get;set;}
        public virtual TableTechnik Technik {get;set;}

    }
}