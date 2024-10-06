using System.ComponentModel.DataAnnotations;

namespace Diesel_modular_application.Models
{
    public class TableRegiony
    {   
        [Key]
        public int IdRegion{get;set;}
        public string NazevRegionu{get;set;}
    }
}