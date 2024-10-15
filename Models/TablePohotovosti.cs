using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocumentFormat.OpenXml.Office.CustomUI;

namespace Diesel_modular_application.Models
{
    public class TablePohotovosti
    {
        [Key]
        public int IdPohotovst{get;set;}
        public DateTime Začátek {get;set;}
        public DateTime Konec {get;set;}

        [ForeignKey("Technik")]
        public string IdTechnik {get;set;}
        public virtual TableTechnici Technik {get;set;}
     
        


    }
}