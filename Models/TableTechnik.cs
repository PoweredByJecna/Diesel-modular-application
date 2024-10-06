using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace Diesel_modular_application.Models
{
    public class TableTechnik
    {
        [Key]
        public int IdTechnika{get;set;}
        public string Jmeno{get;set;}
        public string  Tel{get;set;}

        [ForeignKey("Firma")]
        public int FirmaId{get;set;}
        public virtual TableFirma Firma {get;set;}

         [ForeignKey("Region")]
        public int RegionId {get;set;}
        public virtual TableRegiony Regiony {get;set;}
    }
}