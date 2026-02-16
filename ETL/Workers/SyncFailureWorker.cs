using ETL.Channels;

namespace ETL.Workers;

public class SyncFailureWorker(SyncChannels channels) : BackgroundService
{
    private readonly ILogger<SyncFailureWorker> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in channels.Rejected.Reader.ReadAllAsync(stoppingToken))
        {
            _logger.LogError(
                "Record {Key} rejected:",
                item.SyncKey);
        }
    }
}
