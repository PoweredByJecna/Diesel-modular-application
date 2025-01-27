using System.Diagnostics;
using System.Security.Cryptography;
using Diesel_modular_application.Controllers;
using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.EntityFrameworkCore;

namespace Diesel_modular_application.Services
{
    public class RegionyService
    {
        private readonly DAdatabase _context;
        private readonly OdstavkyService _odstavkyService;

        public RegionyService(DAdatabase context, OdstavkyService odstavkyService)
        {
            _context=context;
            _odstavkyService = odstavkyService;
        }
        public bool TechnikHasPohotovost(string idTechnik)
        {
            var exists = _context.Pohotovts
                .Any(p => p.IdTechnik == idTechnik);
            return exists; 
        }
            
        
        public async Task<List<object>> GetRegionDataPrahaAsync()
        {
            var _IdRegionu= 4;

            var _pocetLokalit = await _context.LokalityS
            .Include(o=>o.Region)
            .Where(o=>o.Region.IdRegion== _IdRegionu)
            .CountAsync();

            var _pocetOdstavek = await _context.OdstavkyS
            .Include(o=>o.Lokality)
            .ThenInclude(o=>o.Region)
            .Where(o=>o.Lokality.Region.IdRegion== _IdRegionu)
            .CountAsync();

            var regiony = await _context.ReginoS
            .Include(o => o.Firma)
            .Where(o => o.IdRegion == _IdRegionu)
            .ToListAsync();

            var resultList = new List<object>();

            foreach (var reg in regiony)
            {
            
                var technikList = await _context.TechniS
                    .Where(t => t.FirmaId == reg.Firma.IDFirmy)
                    .ToListAsync();

                var techniciDto = new List<object>();

                foreach (var t in technikList)
                {
                    bool _maPohotovost = TechnikHasPohotovost(t.IdTechnika);

                    techniciDto.Add(new {
                    jmeno = $"{t.Jmeno} {t.Prijmeni}",
                    maPohotovost = _maPohotovost
                    });
                }


                var regionData = new 
                {
                    firma = reg.Firma.NazevFirmy,
                    distributor = _odstavkyService.DetermineDistributor(reg.NazevRegionu),
                    pocetLokalit = _pocetLokalit,
                    pocetOdstavek = _pocetOdstavek,
                    technici = techniciDto 
                };

                resultList.Add(regionData);
            }
            return resultList.Cast<object>().ToList();
                
        }
    }

}