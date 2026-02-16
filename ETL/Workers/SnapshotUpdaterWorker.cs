using ETL.Channels;
using ETL.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
using static Dapper.SqlMapper;

namespace ETL.Workers;

public class SnapshotUpdaterWorker(SyncChannels channels, IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var entity in channels.Accepted.Reader.ReadAllAsync(stoppingToken))
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext.ApplicationDbContext>();
            var writers = scope.ServiceProvider.GetServices<ISyncEntityWriter>();

            var writer = writers.First(w => w.CanHandle(entity.GetType()));
            writer.Add(db, entity);

            await db.SaveChangesAsync(stoppingToken);
        }
    }
}
