using System.Diagnostics;
using System.Security.Cryptography;
using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using Microsoft.EntityFrameworkCore;
namespace Diesel_modular_application.Services
{
    public class OdstavkyService
    {
        private readonly DAdatabase _context;
        private readonly IDieslovaniService _dieslovani; 
        private readonly LogService _logService;

        public OdstavkyService(DAdatabase context, IDieslovaniService dieslovani, LogService logService)
        {
            _context = context;
            _dieslovani = dieslovani;
            _logService = logService;
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

        public async Task<HandleOdstavkyDieslovaniResult> CreateOdstavkaAsync(string lokalita, DateTime od, DateTime @do, string popis)
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
                result = OdstavkyCheck(lokalitaSearch, od, @do, result);
                if (!result.Success)
                    return result;

                // Který distributor
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                string distrib = DetermineDistributor(lokalitaSearch.Region.NazevRegionu);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                // Vytvoříme novou odstávku
                var newOdstavka = CreateNewOdstavka(lokalitaSearch, distrib, od, @do, popis);

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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var lokalitaSearch = await _context.LokalityS
                    .Include(o => o.Region)
                    .ThenInclude(p => p.Firma)
                    .FirstOrDefaultAsync(i => i.Id == IdNumber);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                if (lokalitaSearch == null)
                {
                    result.Success = false;
                    result.Message = "Lokalita nenalezena.";
                    return result;
                }

                // Generujeme náhodné časy
                var hours = RandomNumberGenerator.GetInt32(1, 100);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                string distrib = DetermineDistributor(lokalitaSearch.Region.NazevRegionu);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                var od = DateTime.Today.AddHours(hours + 2);
                var do_ = DateTime.Today.AddHours(hours + 8);
                string popis = $"Odstávka od {distrib}, od: {od}, do: {do_}";
                


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
                if(newOdstavka!=null && newOdstavka.Lokality!=null && newOdstavka.Lokality.Region!=null)
                {
                    var id= newOdstavka.IdOdstavky;
                    await _logService.ZapisDoLogu(DateTime.Now.Date, "odstávka",id, $"Vytvřáření odstávky s parametry: Lokalita: {newOdstavka.Lokality.Lokalita}, Klasifikace: {newOdstavka.Lokality?.Klasifikace}, Od: {newOdstavka?.Od}, Do: {newOdstavka?.Do}");
                    await _logService.ZapisDoLogu(DateTime.Now.Date, "Odstávka", id,$"Baterie: {newOdstavka?.Lokality?.Baterie} min");
                    
                    result = await _dieslovani.HandleOdstavkyDieslovani(newOdstavka, result);
                    if(!result.Success)
                    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        await _logService.ZapisDoLogu(DateTime.Now.Date, "odstávka", newOdstavka.IdOdstavky, result.Message );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    }
                    else{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        await _logService.ZapisDoLogu(DateTime.Now.Date, "Odstávka", newOdstavka.IdOdstavky, result.Message);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Neočekávaná chyba: {ex.Message}";
                return result;
            }
        }
        public async Task<TableOdstavky?> DetailOdstavkyAsync(int id)
        {
            var detail = await _context.OdstavkyS
            .FirstOrDefaultAsync(o => o.IdOdstavky == id);
            return detail;
        }
        public async Task<object> DetailOdstavkyJsonAsync(int id)
        {
            var detailOdstavky = await _context.OdstavkyS
            .Include(o=>o.Lokality)
            .ThenInclude(o=>o.Region)
            .Where(o=>o.IdOdstavky==id)
            .FirstOrDefaultAsync();
            if (detailOdstavky==null)
            {
                return new{
                    error = "Odstavka nenalezena" 
                };
            }
            else if(detailOdstavky.Lokality==null)
            {
                return new{
                    error = "Odstavka nenalezena" 
                };
            }
            else if(detailOdstavky.Lokality.Region==null)
            {
                return new{
                    error = "Odstavka nenalezena" 
                };
            }


            return new
            {
                odstavkaId=detailOdstavky.IdOdstavky,
                lokalita = detailOdstavky.Lokality.Lokalita,
                adresa = detailOdstavky.Lokality.Adresa,
                klasifikace = detailOdstavky.Lokality.Klasifikace,
                baterie = detailOdstavky.Lokality.Baterie,
                region = detailOdstavky.Lokality.Region.NazevRegionu,
                popis = detailOdstavky.Popis,
        
            };
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


        private static bool IsValidDateRange(DateTime od, DateTime Do)
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

            if (!IsValidDateRange(od, do_))
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
            if(lokalitaSearch != null && lokalitaSearch.Region != null && lokalitaSearch.Region.Firma != null)
            {
                Debug.WriteLine($"Vytváří se odstávka s parametry: {distrib}, {lokalitaSearch.Region.Firma.NazevFirmy}, {od}, {do_}, {popis}, {lokalitaSearch.Id}");

            }
            return newOdstavka;
        }

        public string DetermineDistributor(string NazevRegionu)
        {
            return NazevRegionu switch
            {
                "Severní Čechy" or "Západní Čechy" or "Severní Morava" => "ČEZ",
                "Jižní Morava" or "Jižní Čechy" => "EGD",
                "Praha + Střední Čechy" => "PRE",
                _ => ""
            };
        }


        /// <summary>
        /// Represents the result of handling an odstávka (outage) and its associated dieslování (dieseling).
        /// Contains information about the success of the operation, messages, and related entities.
        /// </summary>
        public class HandleOdstavkyDieslovaniResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = "";
            public TableDieslovani? Dieslovani { get; set; }
            public TableOdstavky? Odstavka { get; set; }
            public string EmailResult{get; set;} ="";
            public string Duvod {get;set;} ="";
        }
    }

}
