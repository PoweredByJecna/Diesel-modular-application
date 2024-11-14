using System.Security.AccessControl;
using System.Security.Cryptography;
using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Diesel_modular_application.KlasifikaceRule;

namespace Diesel_modular_application.Controllers
{
    public class OdstavkyController : Controller
    {
        private readonly DAdatabase _context;

        public OdstavkyController(DAdatabase context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> IndexAsync(OdstavkyViewModel odstavky, int page=1)
        {
            int pagesize=10;
            odstavky.OdstavkyList = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .ToListAsync();
            odstavky.PohotovostList = await _context.Pohotovts
                .Include(o => o.Technik)
                .ToListAsync();
            odstavky.PohotovostList = await _context.Pohotovts
                .Include(O=>O.User)
                .ToListAsync();
            odstavky.TechnikList= await _context.TechniS
                .Include(o=>o.Firma)
                .ToListAsync();
            odstavky.RegionyList=await _context.ReginoS
                .Include(O=>O.Firma)    
                .ToListAsync();
            odstavky.DieslovaniList=await _context.DieslovaniS
                .Include(o=>o.Technik)
                .ToListAsync();

            var odstavkyQuery = _context.OdstavkyS
                .Include(o=>o.Lokality)
                .OrderBy(o=>o.IdOdstavky);    

            odstavky.OdstavkyList= await odstavkyQuery
                .Skip((page-1)*pagesize)
                .Take(pagesize)
                .ToListAsync();

            odstavky.CurrentPage=page;
            odstavky.TotalPages=(int)Math.Ceiling(await odstavkyQuery.CountAsync()/(double)pagesize);    
            return View("Index", odstavky);
        }
        public async Task<IActionResult> Search(OdstavkyViewModel search, string query, int page = 1)
        {
            int pageSize = 10; // nastavte počet záznamů na stránku

            List<TableOdstavky>FilteredList;
            if(string.IsNullOrEmpty(query))
            {
                FilteredList = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                search.OdstavkyList=FilteredList;
            }
            else
            {
                FilteredList = await _context.OdstavkyS
                    .Include(o=>o.Lokality)
                    .Where(o=>o.Lokality.Lokalita.Contains(query))
                    .ToListAsync();
                search.OdstavkyList=FilteredList;
            }
            return PartialView("_OdstavkyListPartial",search);
        }
        public async Task<IActionResult> Create(OdstavkyViewModel odstavky)
        {
            var lokalitaSearch = await _context.LokalityS
                .Include(l => l.Region)
                .ThenInclude(r => r.Firma)
                .FirstOrDefaultAsync(input => input.Lokalita == odstavky.AddOdstavka.Lokality.Lokalita);

            if (lokalitaSearch == null)
            {
                ViewBag.Message = "Zadaná lokalita neexistuje";
                return View("Index", odstavky);
            }
            
            var distrib="";
         
            if(lokalitaSearch.Region.NazevRegionu=="Severní Čechy" || lokalitaSearch.Region.NazevRegionu=="Západní Čechy" || lokalitaSearch.Region.NazevRegionu=="Severní Morava")
            {
                distrib= "ČEZ";
            }
            if(lokalitaSearch.Region.NazevRegionu=="Jižní Morava" || lokalitaSearch.Region.NazevRegionu=="Jižní Čechy")
            {
                distrib= "EGD";
            }
            if(lokalitaSearch.Region.NazevRegionu=="Praha + Střední Čechy")
            {
                distrib= "PRE";
            }
          

            var newOdstavka = new TableOdstavky
            {            
                Distributor = distrib,
                Firma=lokalitaSearch.Region.Firma.NázevFirmy,
                Od = odstavky.AddOdstavka.Od,
                Do = odstavky.AddOdstavka.Do,
                Vstup=odstavky.AddOdstavka.Vstup,
                Odchod=odstavky.AddOdstavka.Odchod,
                Popis = odstavky.AddOdstavka.Popis,
                LokalitaId = lokalitaSearch.Id 
            };
            if(newOdstavka.Od.Date<DateTime.Today && newOdstavka.Od>newOdstavka.Do)
            {
                ModelState.AddModelError(string.Empty,"Špatné datum");
            }
            if(newOdstavka.Od.Date>=DateTime.Today && newOdstavka.Od<newOdstavka.Do)
            {
                _context.OdstavkyS.Add(newOdstavka);
                await _context.SaveChangesAsync();
                ModelState.AddModelError(string.Empty,"Odstávka vytvořena");

                var regionId = lokalitaSearch.Region.IdRegion;

                var firmaVRegionu = await _context.ReginoS
                .Where(r => r.IdRegion == regionId)
                .Select(r => r.Firma)
                .FirstOrDefaultAsync();

              
                var TechnikSearch = await _context.Pohotovts
                .Where(p=>p.Technik.FirmaId==firmaVRegionu.IDFirmy && p.Technik.Taken==false)
                .Select(p=>p.Technik.IdTechnika)
                .FirstOrDefaultAsync();

                

                if (TechnikSearch==null)
                {
                    var technik = await _context.Pohotovts
                    .Where(p=>p.Technik.FirmaId==firmaVRegionu.IDFirmy && p.Technik.Taken==true)
                    .Select(p=>p.Technik)
                    .FirstOrDefaultAsync();

                    TempData["Zprava"] = "Technik: " + technik.Jmeno + " je zabrán"+ 
                    "aktuálně je objednán na lokalitu: " + lokalitaSearch.Lokalita;
                    return Redirect ("/Home/Index");
                   
                }
            
                if(TechnikSearch!=null)
                {
                    var NewDieslovani = new TableDieslovani
                    {
                        Vstup=odstavky.DieslovaniMod.Vstup,
                        Odchod=odstavky.DieslovaniMod.Odchod,
                        IDodstavky=newOdstavka.IdOdstavky,
                        IdTechnik=TechnikSearch,
                        FirmaId=firmaVRegionu.IDFirmy
                    };
                    _context.DieslovaniS.Add(NewDieslovani);
                    await _context.SaveChangesAsync();
                    var technik = await _context.TechniS.FindAsync(TechnikSearch);
                    
                    if(odstavky.AddOdstavka.Od.Date==DateTime.Today)
                    {
                        technik.Taken=true;
                        _context.TechniS.Update(technik);
                        TempData["Zprava"] = "TechnikUpdate";
                    }
                    
                    await _context.SaveChangesAsync();
                    odstavky.DieslovaniList=await _context.DieslovaniS.ToListAsync();
                    odstavky.TechnikList=await _context.TechniS.ToListAsync();
                    TempData["Zprava"] = "Odstavka vytvořena";
                }
               
                

                
                odstavky.OdstavkyList = await _context.OdstavkyS.ToListAsync(); 
            }

            
            return Redirect("/Odstavky/Index");
        }
        public async Task<IActionResult> Vstup(OdstavkyViewModel odstavky)
        { 
            var SetOdstavka=new TableOdstavky
            {   
                Vstup=odstavky.AddOdstavka.Vstup

            };
            _context.OdstavkyS.Add(SetOdstavka);
            await _context.SaveChangesAsync();
            return Redirect("/Home/Index");

        }
        public async Task<IActionResult> Test (OdstavkyViewModel odstavky )
        {
            
            var distrib="";
          
           
            for(int i=1;i<=1;i++)
            {
                var number=await _context.LokalityS.CountAsync();
                var IdNumber=RandomNumberGenerator.GetInt32(1,number);
                var lokalitaSearch = await _context.LokalityS.Include(o=>o.Region).ThenInclude(p=>p.Firma).FirstOrDefaultAsync(i=>i.Id==IdNumber);
                if(await _context.LokalityS.AnyAsync())
                {   
                
                if(lokalitaSearch!=null)
                {
                    if(lokalitaSearch.Region.NazevRegionu=="Severní Čechy" || lokalitaSearch.Region.NazevRegionu=="Západní Čechy" || lokalitaSearch.Region.NazevRegionu=="Severní Morava")
                    {
                        distrib= "ČEZ";
                    }
                    if(lokalitaSearch.Region.NazevRegionu=="Jižní Morava" || lokalitaSearch.Region.NazevRegionu=="Jižní Čechy")
                    {
                        distrib= "EGD";
                    }
                    if(lokalitaSearch.Region.NazevRegionu=="Praha + Střední Čechy")
                    {
                        distrib= "PRE";
                    }
                    var hours = RandomNumberGenerator.GetInt32(1,50);
                    var newOdstavka = new TableOdstavky
                    {            
                        Distributor = distrib,
                        Firma=lokalitaSearch.Region.Firma.NázevFirmy,
                        Od = DateTime.Today.AddHours(hours),
                        Do = DateTime.Today.AddHours(hours+8),
                        Vstup=odstavky.AddOdstavka.Vstup,
                        Odchod=odstavky.AddOdstavka.Odchod,
                        Popis = "Test",
                        LokalitaId = lokalitaSearch.Id 
                    };
                    _context.OdstavkyS.Add(newOdstavka);
                    await _context.SaveChangesAsync();
                    var regionId = lokalitaSearch.Region.IdRegion;

                    var firmaVRegionu = await _context.ReginoS
                    .Where(r => r.IdRegion == regionId)
                    .Select(r => r.Firma)
                    .FirstOrDefaultAsync();

                

                
                    var TechnikSearch = await _context.Pohotovts
                    .Where(p=>p.Technik.FirmaId==firmaVRegionu.IDFirmy && p.Technik.Taken==false)
                    .Select(p=>p.Technik)
                    .FirstOrDefaultAsync();

                    if (TechnikSearch==null)
                    {
                        var dieslovani = await _context.Pohotovts
                        .Where(p=>p.Technik.FirmaId==firmaVRegionu.IDFirmy && p.Technik.Taken==true)
                        .SelectMany(p => _context.DieslovaniS
                        .Where(td => td.Technik.IdTechnika == p.Technik.IdTechnika))
                        .FirstOrDefaultAsync();


                        if(dieslovani.Odstavka.Lokality.Klasifikace.ZiskejVahu()<newOdstavka.Lokality.Klasifikace.ZiskejVahu() && dieslovani.Odstavka.Od.Date.AddHours(2)<DateTime.Now && dieslovani.Odstavka.Lokality.DA=="False")
                        {

                        }

                        TempData["Zprava"] = "Technik: " + dieslovani.Technik.Jmeno + " je zabrán"+ 
                        "aktuálně je objednán na lokalitu: " + lokalitaSearch.Lokalita;
                        return Redirect ("/Home/Index");
                    }
                    if(newOdstavka.Lokality.DA=="False")
                    {
                        if(TechnikSearch!=null)
                        {
                            var NewDieslovani = new TableDieslovani
                            {
                                Vstup=odstavky.DieslovaniMod.Vstup,
                                Odchod=odstavky.DieslovaniMod.Odchod,
                                IDodstavky=newOdstavka.IdOdstavky,
                                IdTechnik=TechnikSearch.IdTechnika,
                                FirmaId=firmaVRegionu.IDFirmy
                            };
                            _context.DieslovaniS.Add(NewDieslovani);
                            await _context.SaveChangesAsync();
                            var technik = await _context.TechniS.FindAsync(TechnikSearch.IdTechnika);
                            
                            if(newOdstavka.Od.Date==DateTime.Today)
                            {
                                technik.Taken=true;
                                _context.TechniS.Update(technik);
                                TempData["Zprava"] = "TechnikUpdate";
                            }
                            await _context.SaveChangesAsync();
                            TempData["Zprava"] = NewDieslovani.Technik.Jmeno + "bude dieslovat";
                        }
                    }
                    else
                    {
                        TempData["Zprava"] = newOdstavka.Lokality.Lokalita + "na této lokalitě je Diesel agregát, není potřeba dieslovat";

                    }
                    }
                }

            }
            
            await _context.SaveChangesAsync();
            return Redirect("/Home/Index");
        }
        public async Task<IActionResult> Odchod(OdstavkyViewModel odstavky)
        {
            var SetOdstavka = new TableOdstavky
            {
                Odchod=odstavky.AddOdstavka.Odchod
            };
            _context.OdstavkyS.Add(SetOdstavka);
            await _context.SaveChangesAsync();
            return Redirect("/Home/Index");
        }
       

        public async Task<IActionResult> Delete (OdstavkyViewModel odstavky)
        {
            
            var odstavka= await _context.OdstavkyS.FindAsync(odstavky.OdstavkyMod.IdOdstavky);
            if(odstavka!=null)
            {
                var dieslovani = await _context.DieslovaniS.Where(p=>p.IDodstavky==odstavka.IdOdstavky).FirstOrDefaultAsync();
                
                if(dieslovani!=null)
                {
                    var technik = await _context.TechniS.Where(p=>p.IdTechnika==dieslovani.IdTechnik).FirstOrDefaultAsync();
                    if(technik!=null)
                    {   
                        technik.Taken=false;
                        _context.TechniS.Update(technik);
                    }
                }

            }
           
            _context.OdstavkyS.Remove(odstavka);
            await _context.SaveChangesAsync();
            return Redirect ("/Odstavky/Index");
        } 
        


    }

}