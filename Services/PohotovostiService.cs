using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Diesel_modular_application.Services
{
    public class PohotovostiService
    {
        private readonly DAdatabase _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PohotovostiService(DAdatabase context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Metoda, která získá seznam všech pohotovostí (pro zobrazení v Index).
        /// </summary>
        public async Task<List<TablePohotovosti>> GetAllPohotovostiAsync()
        {
            return await _context.Pohotovts
                .Include(o => o.Technik)
                .ToListAsync();
        }

        /// <summary>
        /// Metoda, která provádí zapsání nové pohotovosti 
        /// (původně v metodě Zapis) a řeší logiku podle rolí.
        /// </summary>
        /// <param name="pohotovosti">OdstavkyViewModel, který obsahuje PohotovostMod s daty</param>
        /// <param name="currentUser">Aktuálně přihlášený uživatel</param>
        /// <returns>Vrací (bool Success, string Message), abychom věděli, zda se zapsání zdařilo.</returns>
        public async Task<(bool Success, string Message)> ZapisPohotovostAsync(
            TablePohotovosti pohotovosti,
            IdentityUser currentUser)
        {
            // Zjistíme, jaké role má aktuální uživatel
            bool isEngineer = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer");
            bool isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

            if (!isEngineer && !isAdmin)
            {
                return (false, "Nemáte oprávnění zapsat pohotovost.");
            }

            // Validace základního intervalu
            if (pohotovosti.Začátek <= pohotovosti.Začátek ||
                pohotovosti.Začátek < DateTime.Today)
            {
                return (false, "Neplatný interval pohotovosti.");
            }

            if (isEngineer)
            {
                // Najdeme technika podle aktuálního uživatele
                var technikSearch = await _context.TechniS
                    .FirstOrDefaultAsync(input => input.IdUser == currentUser.Id);

                if (technikSearch == null)
                {
                    return (false, "Nepodařilo se najít technika přiřazeného k aktuálnímu uživateli.");
                }

                // Vytvoříme záznam pohotovosti
                var zapis = new TablePohotovosti
                {
                    IdUser = technikSearch.IdUser,
                    Začátek = pohotovosti.Začátek,
                    Konec = pohotovosti.Konec,
                    IdTechnik = technikSearch.IdTechnika
                };

                _context.Pohotovts.Add(zapis);
                await _context.SaveChangesAsync();

                return (true, "Pohotovost byla úspěšně zapsána (engineer).");
            }

            if (isAdmin)
            {
                // Najdeme technika podle ID, které je v pohotovosti.TechnikMod
                var technikSearch = await _context.TechniS
                    .FirstOrDefaultAsync(input => input.IdTechnika == pohotovosti.Technik.IdTechnika);

                if (technikSearch == null)
                {
                    return (false, "Nepodařilo se najít technika podle zadaného IdTechnika.");
                }

                var zapis = new TablePohotovosti
                {
                    IdUser = technikSearch.IdUser,
                    Začátek = pohotovosti.Začátek,
                    Konec = pohotovosti.Konec,
                    IdTechnik = technikSearch.IdTechnika
                };

                _context.Pohotovts.Add(zapis);
                await _context.SaveChangesAsync();

                return (true, "Pohotovost byla úspěšně zapsána (admin).");
            }

            return (false, "Nepodařilo se provést zápis pohotovosti.");
        }

        /// <summary>
        /// Metoda pro naplnění data pro DataTable: GetTableDatapohotovostiTable
        /// </summary>
        public async Task<(int totalRecords, List<object> data)> GetPohotovostTableDataAsync(int start, int length)
        {
            int totalRecords = await _context.Pohotovts.CountAsync();
            if (length == 0)
            {
                length = totalRecords;
            }

            // Vybereme Id techniků, kteří mají pohotovost
            var pohotovostTechnikIds = await _context.Pohotovts
                .Select(p => p.Technik.IdTechnika)
                .Distinct()
                .ToListAsync();

            // Mapa technik -> první (neskončená) lokalita, na které diesluje
            var technikLokalitaMap = await _context.DieslovaniS
                .Include(o => o.Odstavka).ThenInclude(l => l.Lokality)
                .Where(d => pohotovostTechnikIds.Contains(d.IdTechnik))
                .GroupBy(d => d.IdTechnik)
                .ToDictionaryAsync(
                    group => group.Key,
                    group => group
                        .OrderBy(o => o.Odstavka.Od)
                        .Where(o => o.Odstavka.ZadanOdchod == false)
                        .Select(d => d.Odstavka.Lokality.Lokalita)
                        .FirstOrDefault()
                );

            var pohotovostList = await _context.Pohotovts
                .Include(o => o.Technik).ThenInclude(o => o.User)
                .Include(o => o.Technik).ThenInclude(o => o.Firma)
                .OrderBy(o => o.Začátek)
                .Skip(start)
                .Take(length)
                .Select(l => new
                {
                    l.Technik.Jmeno,
                    l.Technik.Prijmeni,
                    PhoneNumber = l.Technik.User.PhoneNumber,
                    Firma = l.Technik.Firma.NázevFirmy,
                    l.Začátek,
                    l.Konec,
                    l.Technik.Taken,
                    Lokalita = technikLokalitaMap.ContainsKey(l.Technik.IdTechnika)
                        ? technikLokalitaMap[l.Technik.IdTechnika]
                        : "Nemá přiřazenou lokalitu"
                })
                .ToListAsync();

            return (totalRecords, pohotovostList.Cast<object>().ToList());
        }
    }
}
