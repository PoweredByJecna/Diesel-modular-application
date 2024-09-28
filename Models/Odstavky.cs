namespace Diesel_modular_application.Models
{
    public class Odstavky:Lokality
    {
        public required int IdOdstavky{get;set;}
        public required string Distributor {get; set;}
        public required DateTime Od {get; set;}
        public required DateTime Do {get; set;}
        public string Popis{get; set;}        
    }       
}       