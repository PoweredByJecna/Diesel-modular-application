using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Diesel_modular_application.Models
{
    public class DebugLogModel
    {
        [Key]
        public int IdLog{ get; set;}
        public DateTime TimeStamp {get; set;}
        public required string EntityName {get; set;}
        public int EntityId{get; set;}
        public required string LogMessage{get;set;}
    }

}