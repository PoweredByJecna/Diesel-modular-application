using System.Diagnostics;
using System.Security.Cryptography;
using Diesel_modular_application.Controllers;
using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using Microsoft.EntityFrameworkCore;
using static Diesel_modular_application.Services.DieslovaniService;

namespace Diesel_modular_application.Services
{
    public class OdstavkyService
    {
        private readonly DAdatabase _context;
        private readonly DieslovaniService _dieslovani; 

        public OdstavkyService(DAdatabase context, DieslovaniService dieslovani)
        {
            _context = context;
            _dieslovani = dieslovani;

        }

        public StatsViewModel GetRegionStats()
        {
            var odstavkyList = _context.OdstavkyS
            .ToList();

            var today = DateTime.Today;

            var regions = new List<string>
            {
                "Západní Čechy",
                "Jižní Čechy",
                "Praha + Střední Čechy",
                "Severní Morava",
                "Jižní Morava",
                "Severní Čechy"
            };

            var totalOdstavky = odstavkyList.Count();

            var regionStats = regions.Select(region => new RegionStats
            {
            RegionName = region,
        
            Count = odstavkyList.Count(o =>
            o.Lokality?.Region?.NazevRegionu == region), 

        
            Percentage = totalOdstavky == 0 ? 0 : (double)odstavkyList.Count(o =>
            o.Lokality?.Region?.NazevRegionu == region) / totalOdstavky * 100,


            StatusColor = totalOdstavky == 0 ? "gray" : "green" // Změňte na podmínky pro jiné barvy podle potřeby
            }).ToList();


            return new StatsViewModel
            {
                TotalOdstavky = totalOdstavky,
                Regions = regionStats
            };
        }

        public async Task<List<string>> SuggestLokalitaAsync(string query)
        {
            var lokalities = await _context.LokalityS
                .Where(l => l.Lokalita.Contains(query))
                .Select(l => l.Lokalita)
                .Take(10)
                .ToListAsync();
            return lokalities;
        }

        public async Task<HandleOdstavkyDieslovaniResult> CreateOdstavkaAsync(string lokalita, DateTime od, DateTime do_, string popis)
        {
            var result = new HandleOdstavkyDieslovaniResult();

            try
            {
                // Najdeme danou lokalitu
                var lokalitaSearch = await _context.LokalityS
                    .Include(l => l.Region)
                    .ThenInclude(r => r.Firma)
                    .FirstOrDefaultAsync(l => l.Lokalita == lokalita);

                if (lokalitaSearch == null)
                {
                    result.Success = false;
                    result.Message = "Lokalita nenalezena.";
                    return result;
                }

                // Kontrola termínů a existence odstávky
                result = OdstavkyCheck(lokalitaSearch, od, do_, result);
                if (!result.Success)
                    return result;

                // Který distributor
                string distrib = DetermineDistributor(lokalitaSearch.Region.NazevRegionu);

                // Vytvoříme novou odstávku
                var newOdstavka = CreateNewOdstavka(lokalitaSearch, distrib, od, do_, popis);

                try
                {
                    _context.OdstavkyS.Add(newOdstavka);
                    await _context.SaveChangesAsync();
                    result.Odstavka = newOdstavka;
                    result.Message = "Odstávka byla úspěšně vytvořena.";
                }
                catch (Exception)
                {
                    result.Success = false;
                    result.Message = "Chyba při ukládání do databáze";
                    return result;
                }

                // Zavoláme dieslování (pokud je potřeba)
                result = await _dieslovani.HandleOdstavkyDieslovani(newOdstavka,result);
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Neočekávaná chyba: {ex.Message}";
                return result;
            }
        }


        public async Task<HandleOdstavkyDieslovaniResult> TestOdstavkaAsync()
        {
            var result = new HandleOdstavkyDieslovaniResult();

            try
            {
                var number = await _context.LokalityS.CountAsync();
                if (number == 0)
                {
                    result.Success = false;
                    result.Message = "Žádné lokality v DB.";
                    return result;
                }

                var IdNumber = RandomNumberGenerator.GetInt32(1, number);
                var lokalitaSearch = await _context.LokalityS
                    .Include(o => o.Region)
                    .ThenInclude(p => p.Firma)
                    .FirstOrDefaultAsync(i => i.Id == IdNumber);

                if (lokalitaSearch == null)
                {
                    result.Success = false;
                    result.Message = "Lokalita nenalezena.";
                    return result;
                }

                // Generujeme náhodné časy
                var hours = RandomNumberGenerator.GetInt32(1, 100);
                string popis = "test";
                string distrib = DetermineDistributor(lokalitaSearch.Region.NazevRegionu);

                var od = DateTime.Today.AddHours(hours + 2);
                var do_ = DateTime.Today.AddHours(hours + 8);

                // Kontrola
                result = OdstavkyCheck(lokalitaSearch, od, do_, result);
                if (!result.Success)
                    return result;

                // Vytvoříme novou odstávku
                var newOdstavka = CreateNewOdstavka(lokalitaSearch, distrib, od, do_, popis);

                try
                {
                    _context.OdstavkyS.Add(newOdstavka);
                    await _context.SaveChangesAsync();
                    result.Odstavka = newOdstavka;
                    result.Message = "Odstávka byla úspěšně vytvořena.";
                }
                catch (Exception)
                {
                    result.Success = false;
                    result.Message = "Chyba při ukládání do databáze";
                    return result;
                }

                // Zavoláme dieslování
                result = await _dieslovani.HandleOdstavkyDieslovani(newOdstavka, result);
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Neočekávaná chyba: {ex.Message}";
                return result;
            }
        }


        public async Task<HandleOdstavkyDieslovaniResult> DeleteOdstavkaAsync(int idodstavky)
        {
            var result = new HandleOdstavkyDieslovaniResult();
            try
            {
                var odstavka = await _context.OdstavkyS.FindAsync(idodstavky);
                if (odstavka == null)
                {
                    result.Success = false;
                    result.Message = "Záznam nebyl nalezen.";
                    return result;
                }

                // Případně zrušení dieslování a uvolnění technika
                var dieslovani = await _context.DieslovaniS
                    .Where(p => p.IDodstavky == odstavka.IdOdstavky)
                    .FirstOrDefaultAsync();
                if (dieslovani != null)
                {
                    var technik = await _context.TechniS
                        .Where(p => p.IdTechnika == dieslovani.IdTechnik)
                        .FirstOrDefaultAsync();
                    if (technik != null)
                    {
                        technik.Taken = false;
                        _context.TechniS.Update(technik);
                    }
                }

                _context.OdstavkyS.Remove(odstavka);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Záznam byl úspěšně smazán.";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Chyba při mazání záznamu: " + ex.Message;
                return result;
            }
        }


        public async Task<(int totalRecords, List<object> data)> GetTableDataAsync(int start = 0, int length = 0)
        {
            int totalRecords = await _context.OdstavkyS.CountAsync();
            // Pokud chceme prostě vypsat vše, tak je length = totalRecords
            if (length == 0) 
                length = totalRecords;

            var odstavkyList = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .OrderBy(o => o.Od)
                .Skip(start)
                .Take(length)
                .Select(l => new
                {
                    l.IdOdstavky,
                    l.Distributor,
                    l.ZadanOdchod,
                    l.ZadanVstup,
                    l.Lokality.Lokalita,
                    l.Lokality.Klasifikace,
                    l.Od,
                    l.Do,
                    l.Lokality.Adresa,
                    l.Lokality.Baterie,
                    l.Popis,
                    l.Lokality.Zasuvka,
                    idTechnika = _context.DieslovaniS
                        .Where(d => d.IDodstavky == l.IdOdstavky)
                        .Select(d => d.Technik.IdTechnika)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return (totalRecords, odstavkyList.Cast<object>().ToList());
        }


        public async Task<List<object>> GetTableDataOdDetailAsync(int dieslovaniId)
        {
            var odstavkaList = await _context.DieslovaniS
                .Include(o => o.Odstavka)
                    .ThenInclude(o => o.Lokality)
                .Where(i => i.IdDieslovani == dieslovaniId)
                .Select(o => new
                {
                    idOdstavky = o.Odstavka.IdOdstavky,
                    distributor = o.Odstavka.Distributor,
                    lokalita = o.Odstavka.Lokality.Lokalita,
                    klasifikace = o.Odstavka.Lokality.Klasifikace,
                    od = o.Odstavka.Od,
                    do_ = o.Odstavka.Do,
                    adresa = o.Odstavka.Lokality.Adresa,
                    baterie = o.Odstavka.Lokality.Baterie,
                    popis = o.Odstavka.Popis,
                    zasuvka = o.Odstavka.Lokality.Zasuvka
                })
                .ToListAsync();

            return odstavkaList.Cast<object>().ToList();
        }


        private bool ISvalidDateRange(DateTime od, DateTime Do)
        {
            return od.Date >= DateTime.Today && od < Do;
        }


        private bool ExistingOdstavka(int lokalitaSearchId, DateTime od)
        {
            var existingOdstavka = _context.OdstavkyS
                .FirstOrDefault(o => o.Od.Date == od.Date && o.Lokality.Id == lokalitaSearchId);

            if (existingOdstavka == null)
            {
                return true; 
            }
            else
            {
                Debug.WriteLine($"Odstávka je již založena: {existingOdstavka.IdOdstavky} na tento den: {existingOdstavka.Od.Date}");
                return false;
            }
        }


        private HandleOdstavkyDieslovaniResult OdstavkyCheck(TableLokality lokalitaSearch, DateTime od, DateTime do_, HandleOdstavkyDieslovaniResult result)
        {
            if (!ExistingOdstavka(lokalitaSearch.Id, od))
            {
                result.Success = false;
                result.Message = "Již existuje jiná odstávka.";
                return result;
            }

            if (!ISvalidDateRange(od, do_))
            {
                result.Success = false;
                result.Message = "Špatně zadané datum.";
                return result;
            }
            else
            {
                result.Success = true;
                return result;
            }
        }


        private TableOdstavky CreateNewOdstavka(TableLokality lokalitaSearch, string distrib, DateTime od, DateTime do_, string popis)
        {
            var newOdstavka = new TableOdstavky
            {
                Distributor = distrib,
                Od = od,
                Do = do_,
                Popis = popis,
                LokalitaId = lokalitaSearch.Id
            };
            Debug.WriteLine($"Vytváří se odstávka s parametry: {distrib}, {lokalitaSearch.Region.Firma.NazevFirmy}, {od}, {do_}, {popis}, {lokalitaSearch.Id}");
            return newOdstavka;
        }

        private string DetermineDistributor(string NazevRegionu)
        {
            return NazevRegionu switch
            {
                "Severní Čechy" or "Západní Čechy" or "Severní Morava" => "ČEZ",
                "Jižní Morava" or "Jižní Čechy" => "EGD",
                "Praha + Střední Čechy" => "PRE",
                _ => ""
            };
        }


        public class HandleOdstavkyDieslovaniResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = "";
            public TableDieslovani? Dieslovani { get; set; }
            public TableOdstavky? Odstavka { get; set; }
            public string EmailResult{get; set;} ="";
        }
    }

}
