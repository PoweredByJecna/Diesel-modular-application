using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Diesel_modular_application.Data;

namespace Diesel_modular_application.Services
{
    public class CleaningDatabase: IHostedService, IDisposable
    {  
    
        
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _Timer;

        public CleaningDatabase(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _Timer = new Timer(DoWork,null,TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
            
                var _context = scope.ServiceProvider.GetRequiredService<DAdatabase>();
                var outdatedRecordsOdstavky= await _context.OdstavkyS
                .Where(d=>d.Do.Date<DateTime.Today).ToListAsync();

                var outdatedRecordsDieslovani = await _context.DieslovaniS.Include(d=>d.Technik)
                .Where(d=>d.Odstavka.Do.Date<DateTime.Today).ToListAsync();

                if(outdatedRecordsOdstavky.Any())
                {
                    
                    _context.OdstavkyS.RemoveRange(outdatedRecordsOdstavky);

                    await _context.SaveChangesAsync();
                }
                foreach (var dieslovani in outdatedRecordsDieslovani)
                {
                    if (dieslovani.Technik != null)
                    {
                        dieslovani.Technik.Taken = false;
                
                        _context.TechniS.Update(dieslovani.Technik);
                        
                         await _context.SaveChangesAsync();
                    }
                }
               


            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _Timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _Timer?.Dispose();
        }

    }
}