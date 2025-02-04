using System;
using System.ComponentModel.DataAnnotations;

namespace Diesel_modular_application.Models;

public class TableZdroj
{
    [Key]
    public int Id { get; set; }
    public string Nazev { get; set; } 
    public double Odber { get; set; }   
}
