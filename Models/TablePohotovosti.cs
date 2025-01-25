using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Diesel_modular_application.Models
{
    public class TablePohotovosti
    {
        [Key]
        public int IdPohotovst{get;set;}
        public DateTime Zacatek {get;set;}
        public DateTime Konec {get;set;}

        [ForeignKey("user")]
        public string IdUser{get;set;}
        public virtual IdentityUser User {get;set;}

        [ForeignKey("Technik")]
        public string IdTechnik {get;set;}
        public virtual TableTechnici Technik {get;set;}       



    }
}