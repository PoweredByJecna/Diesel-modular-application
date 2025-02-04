using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Diesel_modular_application.Services
{
    public class UserService(DAdatabase context)
    {
        private readonly DAdatabase _context = context;

        public async Task<object> DetailUserJsonAsync(string IdUSER)
        {
            var userDetail = await _context.Users
            .Where(o => o.Id == IdUSER)
            .FirstOrDefaultAsync();

           

            if (userDetail == null)
            {
            return new { error = "Uživatel nenalezen" };
            }

            var role = await _context.UserRoles
            .Where(o=>o.UserId == userDetail.Id)
            .FirstOrDefaultAsync();

            var pohotovost = await _context.Pohotovts
            .Include(o => o.Technik)
            .ThenInclude(o => o.User)
            .Where(o => o.User.Id == userDetail.Id)
            .FirstOrDefaultAsync();

            var Firma = await _context.TechniS
            .Include(p => p.Firma)
            .Where(o => o.IdUser == userDetail.Id)
            .FirstOrDefaultAsync();

            var Region = Firma?.Firma != null
            ? await _context.ReginoS
            .Include(o => o.Firma)
            .Where(o => o.FirmaID == Firma.FirmaId)
            .FirstOrDefaultAsync()
            : null;
            
            return new
            {
                uzivatelskeJmeno = userDetail.UserName,
                stav = pohotovost != null,
                nadrizeny = "Neznámý", 
                firma = Firma?.Firma?.NazevFirmy ?? "Neznámá",
                region = Region?.NazevRegionu ?? "Neznámý",
                jmeno= pohotovost?.Technik?.Jmeno,
                prijmeni=pohotovost?.Technik?.Prijmeni,
                tel=userDetail.PhoneNumber,
                role = role?.RoleId,
    
                
            };
        }

    }
}