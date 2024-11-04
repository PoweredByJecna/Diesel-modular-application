using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
                var outdatedRecords= await _context.DieslovaniS
                .Where(d=>d.Do.Date<Date.Today.AddDays(-5)).ToListAsync();

                if(outdatedRecords.AnyAsync())
                {
                    _context.DieslovaniS.Remove(outdatedRecords);
                    await _context.SaveChangesAsync();
                }


            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

    }
}