using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diesel_modular_application.Controllers
{
    public class DieslovaniController:Controller
    {
        private readonly DAdatabase _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DieslovaniController(DAdatabase context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        public async Task<IActionResult> Vstup (OdstavkyViewModel dieslovani)
        {
           var dis= await _context.DieslovaniS.FindAsync(dieslovani.DieslovaniMod.IdDieslovani);
           if(dis !=null)
           {    
                
                dis.Vstup=DateTime.Now;
                _context.Update(dis);
                await _context.SaveChangesAsync();
                ViewBag.Message="vstup";
           }
            var odstavka = await _context.OdstavkyS.FindAsync(dis.IDodstavky);
            if (odstavka != null)
            {
                // Nastav ZadanVstup na true
                odstavka.ZadanVstup = true;
                _context.Update(odstavka);
                
            }    
               
          
            await _context.SaveChangesAsync();
          return Redirect ("/Home/Index");
        }
        public async Task<IActionResult> Odchod (OdstavkyViewModel dieslovani)
        {
           var dis= await _context.DieslovaniS.FindAsync(dieslovani.DieslovaniMod.IdDieslovani);
           var technik=await _context.TechniS.FindAsync();
           if(dis !=null && technik!=null)
           {    
                technik.Taken=false;
                dis.Odchod=DateTime.Now;
                 _context.Update(dis);
                ViewBag.Message="vstup";
           }
               var odstavka = await _context.OdstavkyS.FindAsync(dis.IDodstavky);
            if (odstavka != null)
            {
                // Nastav ZadanVstup na true
                odstavka.ZadanOdchod=true;
                odstavka.ZadanVstup=false;
               
                _context.Update(odstavka);
                
            }    
            await _context.SaveChangesAsync();
          return Redirect ("/Home/Index");
            
        }
        public async Task<IActionResult> Delete (OdstavkyViewModel dieslovani)
        {
            
             var dis= await _context.DieslovaniS.FindAsync(dieslovani.DieslovaniMod.IdDieslovani);
             _context.DieslovaniS.Remove(dis);
            await _context.SaveChangesAsync();
            return Redirect ("/Home/Index");
        } 
    }
}