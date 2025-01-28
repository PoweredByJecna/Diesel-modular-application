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
        public async Task<List<object>> GetRegionDataPrahaAsync()
        {
            var _IdRegionu= 4;
            var resultList = await GetData(_IdRegionu);
            return resultList;
        }
        public async Task<List<object>> GetRegionDataSeverniMoravaAsync()
        {
            var _IdRegionu=3;
            var resultList = await GetData(_IdRegionu);
            return resultList;
        }
        public async Task<List<object>> GetRegionDataJizniMoravaAsync()
        {
            var _IdRegionu=2;
            var resultList = await GetData(_IdRegionu);
            return resultList;
        }
        public async Task<List<object>> GetRegionDataJizniCechyAsync()
        {
            var _IdRegionu=5;
            var resultList = await GetData(_IdRegionu);
            return resultList;
        }
        public async Task<List<object>> GetRegionDataSeverniCechyAsync()
        {
            var _IdRegionu=1;
            var resultList = await GetData(_IdRegionu);
            return resultList;
        }
        public async Task<List<object>> GetRegionDataZapadniCechyAsync()
        {
            var _IdRegionu=6;
            var resultList = await GetData(_IdRegionu);
            return resultList;
        }

        private bool TechnikHasPohotovost(string idTechnik)
        {
            var exists = _context.Pohotovts
                .Any(p => p.IdTechnik == idTechnik);
            return exists; 
        }
        private async Task<List<object>> GetData(int _IdRegionu)
        {
            var dataList = new List<object>();
            bool _maPohotovost=false;    

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
                    _maPohotovost = TechnikHasPohotovost(t.IdTechnika);

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