using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Diesel_modular_application.Data;
using Diesel_modular_application.KlasifikaceRule;
using Diesel_modular_application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Diesel_modular_application.Services; // Pokud váš EmailController bude též přepsán na EmailService

// Propojení s OdstavkyService.Result, pokud to sdílíte
using static Diesel_modular_application.Services.OdstavkyService;

namespace Diesel_modular_application.Services
{
    public class DieslovaniService
    {
        private readonly DAdatabase _context;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailService _emailService; 
        // ideálně by i EmailController byl nahrazen EmailService

        public DieslovaniService(
            DAdatabase context,
            UserManager<IdentityUser> userManager,
            EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        /// <summary>
        /// Metoda, která řeší veškerou logiku dieslování pro novou odstávku.
        /// Volána z OdstavkyService (původně přímo z DieslovaniController).
        /// </summary>
        public async Task<HandleOdstavkyDieslovaniResult> HandleOdstavkyDieslovani(TableOdstavky newOdstavka,HandleOdstavkyDieslovaniResult result)
        {
            // Pokud na lokalitě existuje stacionární generátor (DA), není potřeba
            if (newOdstavka.Lokality.DA == true)
            {
                Debug.WriteLine("Na lokalitě je DA");
                result.Success = false;
                result.Message = "Na lokalitě se nachází stacionární generátor.";
                return result;
            }

            // Pokud není zásuvka, tak dieslování nemá smysl
            if (newOdstavka.Lokality.Zasuvka == false)
            {
                Debug.WriteLine("Na lokalitě není zásuvka");
                result.Success = false;
                result.Message = "Na lokalitě není zásuvka.";
                return result;
            }

            // Zjistíme, zda je vůbec potřeba diesel
            if (IsDieselRequired(
                    newOdstavka.Lokality.Klasifikace,
                    newOdstavka.Od,
                    newOdstavka.Do,
                    newOdstavka.Lokality.Baterie))
            {
                var technikSearch = await AssignTechnikAsync(newOdstavka);

                if (technikSearch == null)
                {
                    result.Success = false;
                    Debug.WriteLine("Nepodařilo se najít technika");
                    result.Message = "Nepodařilo se přiřadit technika.";
                    return result;
                }
                else
                {
                    // Vytvoříme novou záznam v DieslovaniS
                    var dieslovani = await CreateNewDieslovaniAsync(newOdstavka, technikSearch);
                    result.Dieslovani = dieslovani;

                    var EmailResult="DA-ok";

                    _emailService.SendDieslovaniEmailAsync(dieslovani, EmailResult);

                    result.Message = "Dieslování bylo úspěšně vytvořeno.";
                }
            }
            else
            {
                result.Success = false;
                result.Message = "Dieslování není potřeba";
                Debug.WriteLine("Dieslování není potřeba");
                return result;
            }

            result.Success = true;
            return result;
        }

        /// <summary>
        /// Vstup na lokalitu
        /// </summary>
        public async Task<(bool Success, string Message)> VstupAsync(int idDieslovani)
        {
            try
            {
                var dis = await _context.DieslovaniS
                    .Include(d => d.Technik)
                    .Include(d => d.Odstavka)
                    .FirstAsync(d => d.IdDieslovani == idDieslovani);

                if (dis != null)
                {
                    dis.Vstup = DateTime.Now;
                    dis.Technik.Taken = true;
                    _context.DieslovaniS.Update(dis);

                    // Značka v odstávce
                    var odstavka = await _context.OdstavkyS.FindAsync(dis.Odstavka.IdOdstavky);
                    if (odstavka != null)
                    {
                        odstavka.ZadanVstup = true;
                        _context.OdstavkyS.Update(odstavka);
                    }
                    await _context.SaveChangesAsync();

                    return (true, "Byl zadán vstup na lokalitu.");
                }
                else
                {
                    return (false, "Záznam dieslování nebyl nalezen.");
                }
            }
            catch (Exception ex)
            {
                return (false, "Chyba při zadávání vstupu " + ex.Message);
            }
        }

        /// <summary>
        /// Odchod z lokality
        /// </summary>
        public async Task<(bool Success, string Message)> OdchodAsync(int idDieslovani)
        {
            try
            {
                var dis = await _context.DieslovaniS
                    .Include(d => d.Technik)
                    .Include(d => d.Odstavka)
                    .FirstAsync(d => d.IdDieslovani == idDieslovani);

                if (dis != null)
                {
                    var odstavka = await _context.OdstavkyS
                        .FindAsync(dis.Odstavka.IdOdstavky);
                    if (odstavka != null)
                    {
                        odstavka.ZadanOdchod = true;
                        odstavka.ZadanVstup = false;
                        _context.Update(odstavka);
                    }

                    // Zkontrolujeme, zda technik ještě někde "diesluje"
                    var anotherDiesel = await _context.DieslovaniS
                        .Include(o => o.Odstavka)
                        .Include(o => o.Technik)
                        .Where(o => o.Technik.IdTechnika == dis.Technik.IdTechnika && o.Odstavka.ZadanOdchod == false)
                        .FirstOrDefaultAsync();

                    if (anotherDiesel == null)
                    {
                        // Pokud nemá jinou rozdělanou zakázku, tak je volný
                        dis.Technik.Taken = false;
                    }

                    dis.Odchod = DateTime.Now;
                    _context.Update(dis);

                    await _context.SaveChangesAsync();
                    return (true, "Byl zadán odchod z lokality.");
                }
                else
                {
                    return (false, "Záznam dieslování nebyl nalezen.");
                }
            }
            catch (Exception ex)
            {
                return (false, "Chyba při zadávání odchodu " + ex.Message);
            }
        }

        /// <summary>
        /// Dočasně uvolní/obsadí technika (TemporaryLeave)
        /// </summary>
        public async Task<(bool Success, string Message)> TemporaryLeaveAsync(int idDieslovani)
        {
            try
            {
                var dis = await _context.DieslovaniS
                    .Include(d => d.Technik)
                    .FirstAsync(d => d.IdDieslovani == idDieslovani);

                // Přepneme stav taken
                dis.Technik.Taken = !dis.Technik.Taken;
                _context.Update(dis);
                await _context.SaveChangesAsync();

                return (true, $"Změněn stav technika (Taken = {!dis.Technik.Taken}).");
            }
            catch (Exception ex)
            {
                return (false, "Chyba při dočasném uvolnění: " + ex.Message);
            }
        }

        /// <summary>
        /// Převzetí dieslování (metoda Take)
        /// </summary>
        public async Task<(bool Success, string Message, string? TempMessage)> TakeAsync(int idDieslovani, IdentityUser currentUser)
        {
            try
            {
                // Najdeme, kdo je přihlášen
                var technik = await _context.TechniS
                    .FirstAsync(d => d.IdUser == currentUser.Id);

                var dieslovaniTaken = await _context.DieslovaniS
                    .Include(d => d.Technik)
                    .FirstOrDefaultAsync(d => d.IdDieslovani == idDieslovani);

                if (dieslovaniTaken == null)
                {
                    return (false, "Záznam dieslování nebyl nalezen.", null);
                }

                // Musí mít pohotovost?
                var pohotovostTechnik = await _context.Pohotovts
                    .Include(t => t.Technik)
                    .Where(p => p.Technik.IdTechnika == technik.IdTechnika)
                    .Select(p => p.Technik)
                    .AnyAsync();

                if (pohotovostTechnik == false)
                {
                    return (false, "Nejste zapsán v pohotovosti.", null);
                }

                if (technik.Taken == true)
                {
                    return (false, "Již máte převzaté jiné dieslování.", null);
                }

                // Přiřadíme technika
                dieslovaniTaken.Technik = technik;
                technik.Taken = true;
                _context.Update(technik);
                _context.Update(dieslovaniTaken);
                await _context.SaveChangesAsync();

                // Můžeme vrátit i nějakou "temp" message
                return (true, $"Lokalitu si převzal: {dieslovaniTaken.Technik.Jmeno} {dieslovaniTaken.Technik.Prijmeni}", 
                             "Dieslování bylo úspěšně zadáno.");
            }
            catch (Exception ex)
            {
                return (false, $"Chyba při převzetí: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Detail Dieslovani
        /// </summary>
        public async Task<TableDieslovani?> DetailDieslovaniAsync(int id)
        {
            var detail = await _context.DieslovaniS
            .FirstOrDefaultAsync(o => o.IdDieslovani == id);

            return detail;
        }
        public async Task<object> DetailDieslovaniJsonAsync(int id)
        {
            var detailDieslovani =  await _context.DieslovaniS
                .Include(o => o.Odstavka)
                    .ThenInclude(o => o.Lokality)
                        .ThenInclude(o => o.Region)
                .Include(p => p.Technik)
                .Where(o=>o.IdDieslovani==id).FirstOrDefaultAsync();

            return new 
            {
                idDieslovani = detailDieslovani.IdDieslovani,
                odstavkaId = detailDieslovani.Odstavka?.IdOdstavky,
                lokalita = detailDieslovani.Odstavka?.Lokality?.Lokalita,
                adresa = detailDieslovani.Odstavka?.Lokality?.Adresa,
                klasifikace = detailDieslovani.Odstavka?.Lokality?.Klasifikace,
                baterie = detailDieslovani.Odstavka?.Lokality?.Baterie,
                region = detailDieslovani.Odstavka?.Lokality?.Region?.NazevRegionu,
                technik = detailDieslovani.Technik != null ? $"{detailDieslovani.Technik.Jmeno} {detailDieslovani.Technik.Prijmeni}" : "Neznámý"
            }; 
        }


        /// <summary>
        /// Smaže záznam v DieslovaniS
        /// </summary>
        public async Task<(bool Success, string Message)> DeleteDieslovaniAsync(int iDdieslovani)
        {
            try
            {
                var dieslovani = await _context.DieslovaniS.FindAsync(iDdieslovani);
                if (dieslovani == null)
                {
                    return (false, "Záznam nebyl nalezen.");
                }

                var technik = await _context.TechniS
                    .Where(p => p.IdTechnika == dieslovani.IdTechnik)
                    .FirstOrDefaultAsync();
                _context.DieslovaniS.Remove(dieslovani);

                if (technik != null)
                {
                    // Zkontrolujeme, zda technik ještě někde "diesluje"
                    var anotherDiesel = await _context.DieslovaniS
                        .Include(o => o.Technik)
                        .Where(o => o.Technik.IdTechnika == technik.IdTechnika)
                        .Where(o => o.IdDieslovani != dieslovani.IdDieslovani)
                        .FirstOrDefaultAsync();

                    if (anotherDiesel != null)
                    {
                        technik.Taken = true;
                        Debug.WriteLine("Technik je přivázán k:" + anotherDiesel.IdDieslovani);
                    }
                    else
                    {
                        technik.Taken = false;
                        Debug.WriteLine("Technik nikde nediesluje");
                    }
                    _context.TechniS.Update(technik);
                }

                await _context.SaveChangesAsync();

                var EmailResult="da-del";
                 _emailService.SendDieslovaniEmailAsync(dieslovani, EmailResult);

                return (true, "Záznam byl úspěšně smazán.");
            }
            catch (Exception ex)
            {
                return (false, "Chyba při mazání záznamu: " + ex.Message);
            }
        }

        /// <summary>
        /// Vrátí data pro tabulku (running) - původně GetTableDataRunningTable
        /// </summary>
        public async Task<(int totalRecords, List<object> data)> GetTableDataRunningTableAsync(IdentityUser? currentUser, bool isEngineer)
        {
            // Získáme query s joiny
            var query = _context.DieslovaniS
                .Include(o => o.Odstavka).ThenInclude(o => o.Lokality).ThenInclude(o => o.Region)
                .Include(t => t.Technik).ThenInclude(t => t.Firma)
                .Include(t => t.Technik).ThenInclude(t => t.User)
                .AsQueryable();

            query = await FilteredDataAsync(query, currentUser, isEngineer);

            int totalRecords = await query.CountAsync();

            var DieslovaniRunningList = await query
                .Include(o => o.Odstavka)
                .ThenInclude(o => o.Lokality)
                .Include(t => t.Technik)
                .Where(i => i.Odstavka.ZadanVstup == true)
                .OrderBy(o => o.Odstavka.Od)
                // .Skip(start).Take(length) // pokud chcete stránkovat, do metody si předávejte i start, length
                .Select(l => new
                {
                    l.IdDieslovani,
                    l.Odstavka.Distributor,
                    l.Odstavka.Lokality.Lokalita,
                    l.Odstavka.Lokality.Klasifikace,
                    l.Technik.Jmeno,
                    l.Technik.Prijmeni,
                    l.Vstup,
                    l.Odstavka.Popis,
                    l.Odstavka.Lokality.Baterie,
                    l.Odstavka.Lokality.Zasuvka
                })
                .ToListAsync();

            return (totalRecords, DieslovaniRunningList.Cast<object>().ToList());
        }

        /// <summary>
        /// Obecný filtr pro dotazy - pokud je Engineer, vyfiltruje jen data pro daného uživatele
        /// </summary>
        private async Task<IQueryable<TableDieslovani>> FilteredDataAsync(
            IQueryable<TableDieslovani> query,
            IdentityUser? currentUser,
            bool isEngineer)
        {
            if (currentUser == null) 
                return query;

            if (isEngineer)
            {
                // Najdeme si userId
                var userId = currentUser.Id;
                query = query.Where(d => d.Technik.User.Id == userId);
            }

            return query;
        }

        /// <summary>
        /// Např. pro tabulku "GetTableDataAllTable"
        /// </summary>
        public async Task<(int totalRecords, List<object> data)> GetTableDataAllTableAsync(IdentityUser? currentUser, bool isEngineer)
        {
            var query = _context.DieslovaniS
                .Include(o => o.Odstavka).ThenInclude(o => o.Lokality).ThenInclude(o => o.Region)
                .Include(t => t.Technik).ThenInclude(t => t.Firma)
                .Include(t => t.Technik).ThenInclude(t => t.User)
                .AsQueryable();

            query = await FilteredDataAsync(query, currentUser, isEngineer);

            int totalRecords = await query.CountAsync();

            var DieslovaniList = await query
                .OrderBy(o => o.Odstavka.Od)
                .Select(l => new
                {
                    l.IdDieslovani,
                    l.Odstavka.Distributor,
                    l.Odstavka.Lokality.Lokalita,
                    l.Odstavka.Lokality.Klasifikace,
                    l.Odstavka.Lokality.Adresa,
                    l.Technik.Firma.NazevFirmy,
                    l.Technik.Jmeno,
                    l.Technik.Prijmeni,
                    l.Odstavka.ZadanVstup,
                    l.Odstavka.ZadanOdchod,
                    l.Technik.IdTechnika,
                    l.Odstavka.Lokality.Region.NazevRegionu,
                    l.Odstavka.Od,
                    l.Odstavka.Do,
                    l.Vstup,
                    l.Odchod,
                    l.Odstavka.Popis,
                    l.Odstavka.Lokality.Baterie,
                    l.Odstavka.Lokality.Zasuvka
                })
                .ToListAsync();

            return (totalRecords, DieslovaniList.Cast<object>().ToList());
        }

        /// <summary>
        /// Např. pro tabulku GetTableDatathrashTable
        /// </summary>
        public async Task<(int totalRecords, List<object> data)> GetTableDatathrashTableAsync(IdentityUser? currentUser, bool isEngineer)
        {
            // Najdeme technika pro currentUser
            var technik = await _context.TechniS
                .Include(t => t.Firma)
                .FirstOrDefaultAsync(t => t.IdUser == currentUser.Id);

            var firmaId = technik?.Firma?.IDFirmy;

            // Které regiony patří do téhle firmy
            var validRegions = await _context.ReginoS
                .Where(r => r.Firma.IDFirmy == firmaId)
                .Select(r => r.IdRegion)
                .ToListAsync();

            var query = _context.DieslovaniS
                .Include(o => o.Odstavka)
                    .ThenInclude(o => o.Lokality)
                    .ThenInclude(o => o.Region)
                .Include(t => t.Technik).ThenInclude(t => t.Firma)
                .AsQueryable();

            if (isEngineer && validRegions.Any())
            {
                query = query.Where(d => validRegions.Contains(d.Odstavka.Lokality.Region.IdRegion));
            }

            int totalRecords = await query.CountAsync();

            var DieslovaniList = await query
                .Include(o => o.Odstavka)
                .ThenInclude(o => o.Lokality)
                .ThenInclude(o => o.Region)
                .Include(t => t.Technik)
                .Where(o => 
                    o.Odstavka.ZadanOdchod == false &&
                    o.Odstavka.ZadanVstup == false &&
                    o.Technik.IdTechnika == "606794494")
                .OrderBy(o => o.Odstavka.Od)
                .Select(l => new
                {
                    l.IdDieslovani,
                    l.Odstavka.Distributor,
                    l.Odstavka.Lokality.Lokalita,
                    l.Odstavka.Lokality.Klasifikace,
                    l.Odstavka.Lokality.Region.Firma.NazevFirmy
                })
                .ToListAsync();

            return (totalRecords, DieslovaniList.Cast<object>().ToList());
        }

        /// <summary>
        /// Např. pro tabulku GetTableUpcomingTable
        /// </summary>
        public async Task<(int totalRecords, List<object> data)> GetTableUpcomingTableAsync(IdentityUser? currentUser, bool isEngineer)
        {
            var query = _context.DieslovaniS
                .Include(o => o.Odstavka).ThenInclude(o => o.Lokality).ThenInclude(o => o.Region)
                .Include(t => t.Technik).ThenInclude(t => t.Firma)
                .Include(t => t.Technik).ThenInclude(t => t.User)
                .AsQueryable();

            query = await FilteredDataAsync(query, currentUser, isEngineer);

            int totalRecords = await query.CountAsync();

            var DieslovaniList = await query
                .Include(o => o.Odstavka).ThenInclude(o => o.Lokality)
                .Include(t => t.Technik)
                .Where(o =>
                    o.Odstavka.ZadanVstup == false &&
                    o.Odstavka.ZadanOdchod == false &&
                    o.Odstavka.Od.Date == DateTime.Today &&
                    o.Technik.IdTechnika != "606794494")
                .OrderBy(o => o.Odstavka.Od)
                .Select(l => new
                {
                    l.IdDieslovani,
                    l.Odstavka.Distributor,
                    l.Odstavka.Lokality.Lokalita,
                    l.Odstavka.Lokality.Klasifikace,
                    l.Technik.Jmeno,
                    l.Technik.Prijmeni,
                    l.Odstavka.Od.AddHours(2).Date, 
                    l.Odstavka.Popis,
                    l.Odstavka.Lokality.Baterie,
                    l.Odstavka.Lokality.Zasuvka,
                })
                .ToListAsync();

            return (totalRecords, DieslovaniList.Cast<object>().ToList());
        }

        /// <summary>
        /// Např. pro tabulku GetTableDataEndTable
        /// </summary>
        public async Task<(int totalRecords, List<object> data)> GetTableDataEndTableAsync(IdentityUser? currentUser, bool isEngineer)
        {
            var query = _context.DieslovaniS
                .Include(o => o.Odstavka).ThenInclude(o => o.Lokality).ThenInclude(o => o.Region)
                .Include(t => t.Technik).ThenInclude(t => t.Firma)
                .Include(t => t.Technik).ThenInclude(t => t.User)
                .AsQueryable();

            query = await FilteredDataAsync(query, currentUser, isEngineer);

            int totalRecords = await query.CountAsync();

            var DieslovaniList = await query
                .Include(o => o.Odstavka).ThenInclude(o => o.Lokality)
                .Include(t => t.Technik)
                .Where(o => o.Odstavka.ZadanOdchod == true && o.Odstavka.ZadanVstup == false)
                .OrderBy(o => o.Odstavka.Od)
                .Select(l => new
                {
                    l.IdDieslovani,
                    l.Odstavka.Distributor,
                    l.Odstavka.Lokality.Lokalita,
                    l.Odstavka.Lokality.Klasifikace,
                    l.Odchod
                })
                .ToListAsync();

            return (totalRecords, DieslovaniList.Cast<object>().ToList());
        }

        // --------------------------------------------------------
        // Následují pomocné (soukromé) metody, které dříve byly v controlleru
        // --------------------------------------------------------
        private bool IsDieselRequired(string klasifikace, DateTime Od, DateTime Do, int baterie)
        {
            var casVypadku = klasifikace.ZiskejCasVypadku();
            var rozdil = (Do - Od).TotalMinutes;

            // Pokud je délka odstávky menší než doba, kterou klasifikace "povoluje" (casVypadku * 60),
            // není třeba diesel
            if (casVypadku * 60 > rozdil)
            {
                Debug.WriteLine($"Lokalita je: {klasifikace} může být {casVypadku}h dole");
                return false;
            }
            else
            {
                // Zkontrolujeme, zda baterie (Battery) vydrží
                if (Battery(Od, Do, baterie))
                {
                    Debug.WriteLine($"Baterie vydrží {baterie} min, čas doby odstávky je: {rozdil} min");
                    return false;
                }
                else
                {
                    Debug.WriteLine($"Dieslovani je potřeba");
                    return true;
                }
            }
        }

        private bool Battery(DateTime od, DateTime do_, int baterie)
        {
            var rozdil = (do_ - od).TotalMinutes;
            if (!int.TryParse(baterie.ToString(), out var baterieMinuty))
                baterieMinuty = 0;

            return rozdil <= baterieMinuty;
        }

        private async Task<TableTechnici?> AssignTechnikAsync(TableOdstavky newOdstavka)
        {
            var firmaVRegionu = await GetFirmaVRegionuAsync(newOdstavka.Lokality.Region.IdRegion);
            if (firmaVRegionu != null)
            {
                Debug.WriteLine($"Vybraná firma: {firmaVRegionu.NazevFirmy}");
                // Najdeme volného technika (který má pohotovost)
                var technikSearch = await _context.Pohotovts
                    .Include(p => p.Technik.Firma)
                    .Where(p => p.Technik.FirmaId == firmaVRegionu.IDFirmy && p.Technik.Taken == false)
                    .Select(p => p.Technik)
                    .FirstOrDefaultAsync();

                // Pokud žádný technik není volný, zkusíme náhradu
                if (technikSearch == null)
                {
                    Debug.WriteLine("Technici jsou obsazeni, nebo nemají pohotovost");
                    
                    // Zkusíme zjistit, zda v regionu má aspoň někdo pohotovost
                    bool nejakyTechnikMaPohotovost = _context.Pohotovts
                        .Include(p => p.Technik.Firma)
                        .Where(p => p.Technik.FirmaId == firmaVRegionu.IDFirmy)
                        .Any();

                    if (nejakyTechnikMaPohotovost)
                    {
                        Debug.WriteLine("alespoň jeden technik v regionu pohotovost má, zkus nahradit.");
                        technikSearch = await CheckTechnikReplacementAsync(newOdstavka);
                        if (technikSearch != null)
                        {
                            Debug.WriteLine($"Technik: {technikSearch.Jmeno}, je nahrazen");
                            return technikSearch;
                        }
                    }

                    // Pokud se to nepovedlo, přiřadíme "fiktivního" technika
                    Debug.WriteLine("Žádný technik nebyl nalezen, bude přiřazen fiktivní");
                    var fiktivniTechnik = await _context.TechniS
                        .FirstOrDefaultAsync(p => p.IdTechnika == "606794494");
                    return fiktivniTechnik;
                }

                Debug.WriteLine($"Technik: {technikSearch.Jmeno}, je zapsán a pojede dieslovat");
                return technikSearch;
            }
            else
            {
                return null;
            }
        }

        private async Task<TableFirma?> GetFirmaVRegionuAsync(int regionId)
        {
            return await _context.ReginoS
                .Where(r => r.IdRegion == regionId)
                .Select(r => r.Firma)
                .FirstOrDefaultAsync();
        }

        private async Task<TableTechnici?> CheckTechnikReplacementAsync(TableOdstavky newOdstavka)
        {
            var technik = await GetHigherPriorityAsync(newOdstavka);
            if (technik == null)
            {
                Debug.WriteLine("Priority je null.");
                return null;
            }
            else
            {
                Debug.WriteLine("Technik byl nalezen, nebo nahrazen");
                return technik;
            }
        }

        private async Task<TableTechnici?> GetHigherPriorityAsync(TableOdstavky newOdstavka)
        {
            var dieslovani = await _context.DieslovaniS
                .Include(o => o.Odstavka).ThenInclude(o => o.Lokality)
                .Include(o => o.Technik).ThenInclude(o => o.Firma)
                .Where(p =>
                    p.Technik.Firma.IDFirmy == newOdstavka.Lokality.Region.Firma.IDFirmy &&
                    p.Technik.Taken == true
                )
                .FirstOrDefaultAsync();

            if (dieslovani == null)
            {
                Debug.WriteLine("Dieslovani nenalezeno");
                return null;
            }
            else
            {
                // Kontrola časové kolize
                if (dieslovani.Odstavka.Do < newOdstavka.Od.AddHours(3) ||
                    newOdstavka.Do < dieslovani.Odstavka.Od.AddHours(3))
                {
                    return dieslovani.Technik;
                }
                else
                {
                    int staraVaha = dieslovani.Odstavka.Lokality.Klasifikace.ZiskejVahu();
                    int novaVaha = newOdstavka.Lokality.Klasifikace.ZiskejVahu();

                    bool maVyssiPrioritu = novaVaha > staraVaha;
                    bool casovyLimit = dieslovani.Odstavka.Od.Date.AddHours(3) < DateTime.Now;
                    bool daPodminka = dieslovani.Odstavka.Lokality.DA == false;

                    if (maVyssiPrioritu && casovyLimit && daPodminka)
                    {
                        Debug.WriteLine("Podmínka pro prioritu splněna");
                        var novyTechnik = await _context.TechniS
                            .FirstOrDefaultAsync(p => p.IdTechnika == "606794494");
                        if (novyTechnik != null)
                        {
                            // Přiřadíme do DB hned novou odstávku
                            await CreateNewDieslovaniAsync(newOdstavka, dieslovani.Technik);
                            dieslovani.Technik = novyTechnik;
                            _context.DieslovaniS.Update(dieslovani);
                            await _context.SaveChangesAsync();
                            Debug.WriteLine($"Přiřazen fiktivní technik na lokalitu: {dieslovani.Odstavka.Lokality.Lokalita}");
                        }
                        return novyTechnik;
                    }
                    else
                    {
                        var novyTechnik = await _context.TechniS
                            .Where(t => t.IdTechnika == "606794494")
                            .FirstOrDefaultAsync();

                        return novyTechnik;
                    }
                }
            }
        }

        /// <summary>
        /// Vytvoření záznamu v tabulce DieslovaniS
        /// </summary>
        public async Task<TableDieslovani> CreateNewDieslovaniAsync(TableOdstavky newOdstavky, TableTechnici technik)
        {
            var NewDieslovani = new TableDieslovani
            {
                Vstup = DateTime.MinValue,
                Odchod = DateTime.MinValue,
                IDodstavky = newOdstavky.IdOdstavky,
                IdTechnik = technik.IdTechnika,
            };
            _context.DieslovaniS.Add(NewDieslovani);
            technik.Taken = true;
            _context.TechniS.Update(technik);

            await _context.SaveChangesAsync();
            Debug.WriteLine($"Dieslovani s technikem: {NewDieslovani.IdDieslovani}");

            return NewDieslovani;
        }
    }
}
