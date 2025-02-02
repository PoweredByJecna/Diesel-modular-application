using DocumentFormat.OpenXml.Office2010.Excel;

namespace Diesel_modular_application.Models
{
    public class DebugLogModel
    {
        public int IdLog{ get; set;}
        public DateTime TimeStamp {get; set;}
        public string EntityName {get; set;}
        public int? EntityId{get; set;}
        public string LogMessage{get;set;}
    }

}