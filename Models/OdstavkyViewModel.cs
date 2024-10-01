namespace Diesel_modular_application.Models
{
    public class OdstavkyViewModel:Odstavky
    {
        public IEnumerable<Odstavky> OdstavkyList {get;set;}
        public Odstavky AddOstavka{get; set;}
    }
}