using Diesel_modular_application.Data;
using Microsoft.EntityFrameworkCore;

namespace Diesel_modular_application.Services
{
    
    public class LogService
    {
        private readonly DAdatabase _context;
     
        public LogService (DAdatabase context)
        {
            _context=context;
        }
        public async Task<object> GetLogByEntity(int id)
        {
            var log = await _context.LogS
            .Where(l => l.EntityId == id)
            .OrderBy(l => l.TimeStamp)
            .ToListAsync();

            return log;
        }

    }
}