using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocumentFormat.OpenXml.Drawing;

namespace Diesel_modular_application.Models
{
    public class TableRegiony
    {   
        [Key]
        public int IdRegion{get;set;}
        public string? NazevRegionu{get;set;}
        
        [ForeignKey("Firma")]
        public int ? FirmaID {get;set;}
        public virtual TableFirma? Firma {get;set;}
    }
}