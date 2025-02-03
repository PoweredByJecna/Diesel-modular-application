using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using Microsoft.EntityFrameworkCore;

namespace Diesel_modular_application.Services
{
    public interface ILogService
    {
        Task LogAsync(DebugLogModel logEntry);
    }
    public class LogService
    {
        private readonly DAdatabase _context;
     
        public LogService (DAdatabase context)
        {
            _context=context;
        }
        public async Task<object> GetLogByEntityAsync(int id)
        {
            var log = await _context.LogS
            .Where(l => l.EntityId == id)
            .OrderBy(l => l.TimeStamp)
            .ToListAsync();

            return log;
        }
        public async Task LogAsync(DebugLogModel logEntry)
        {
            // Nastavíme čas, pokud ještě není nastaven
            if (logEntry.TimeStamp == default)
                logEntry.TimeStamp = DateTime.Now;
 
            _context.LogS.Add(logEntry);
            await _context.SaveChangesAsync();
        }
        public async Task<DebugLogModel>ZapisDoLogu(DateTime datum, string entityname, int entityId, string logmessage)
        {
              var logEntry = new DebugLogModel
                {
                    TimeStamp = datum,
                    EntityName = entityname,
                    EntityId = entityId,
                    LogMessage = logmessage 
                };
                await LogAsync(logEntry);

            return logEntry;    

        }

    }
}