using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Diesel_modular_application.Models{
    public class TableTechnici
    {
        [Key]
        public string IdTechnika{get;set;}
        public string Jmeno{get;set;}

        public string Prijmeni{get;set;}

        public bool Taken{get;set;}=false;

        [ForeignKey("Firma")]
        public int FirmaId{get;set;}
        public virtual TableFirma Firma {get;set;}

        [ForeignKey("user")]
        public string IdUser{get;set;}
        public virtual IdentityUser User {get;set;}
        public virtual ICollection<TableDieslovani>? DieslovaniList {get;set;}
    }

   }