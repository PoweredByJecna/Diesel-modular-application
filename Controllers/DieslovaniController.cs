using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diesel_modular_application.Controllers
{
    public class DieslovaniController:Controller
    {
        private readonly DAdatabase _context;

        public DieslovaniController(DAdatabase context)
        {
            _context = context;
        }

        public async Task<IActionResult> Vstup (OdstavkyViewModel dieslovani)
        {
           var odstavkaSearch = await _context.OdstavkyS.FirstOrDefaultAsync(input => input.IdOdstavky == dieslovani.DieslovaniMod.IDodstavky);
           var firmaSearch= await _context.FrimaS.FirstOrDefaultAsync(input=>input.IDFirmy==dieslovani.FirmaMod.IDFirmy);
           var technikSearch= await _context.TechnikS.FirstOrDefaultAsync(input=>input.IdTechnika==dieslovani.TechnikMod.IdTechnika); 

           var NewDieslovani = new TableDieslovani
           {
                Vstup=dieslovani.DieslovaniMod.Vstup,
                Odchod=dieslovani.DieslovaniMod.Odchod,
                IDodstavky=odstavkaSearch.IdOdstavky,
                FirmaId=firmaSearch.IDFirmy,
               IdTechnik=technikSearch.IdTechnika
           }; 

           odstavkaSearch.ZadanVstup=true;
           _context.DieslovaniS.Add(NewDieslovani);
           await _context.SaveChangesAsync();

          return Redirect ("/Home/Index");
        }
    }
}