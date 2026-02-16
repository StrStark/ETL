using ETL.Entity;
using ETL.Models;
using ETL.WebSocket;

namespace ETL.Services;

public class SyncOrchestrator
{
    private readonly IServiceProvider _sp;
    private readonly IChangeSender _sender;

    public SyncOrchestrator(IServiceProvider sp, IChangeSender sender)
    {
        _sp = sp;
        _sender = sender;
    }

    public async Task ClearSnapShot(string entity)
    {
        using var scope = _sp.CreateScope();
        var services = scope.ServiceProvider.GetServices<IEntitySyncService>();
        var service = services.First(s =>
            s.EntityName.Equals(entity, StringComparison.OrdinalIgnoreCase));
        await service.ClearSnapshot(CancellationToken.None);


    }
    public async Task<Guid> StartAsync(string entity)
    {
        using var scope = _sp.CreateScope();
        var services = scope.ServiceProvider.GetServices<IEntitySyncService>();
        var service = services.First(s =>
            s.EntityName.Equals(entity, StringComparison.OrdinalIgnoreCase));

        var current = await service.LoadCurrentAsync(CancellationToken.None);
        var snapshot = await service.LoadSnapshotAsync(CancellationToken.None);

        var (added, removed) = DiffEngine.Calculate(current, snapshot);

        await _sender.SendAsync(entity, added, removed, CancellationToken.None);

        return Guid.NewGuid();
    }

    public async Task<Guid> StartAllAsync()
    {
        using var scope = _sp.CreateScope();
        var services = scope.ServiceProvider.GetServices<IEntitySyncService>();

        foreach (var s in services)
        {
            await StartAsync(s.EntityName);
        }

        return Guid.NewGuid();
    }
}


public static class DiffEngine
{
    public static (List<ISyncEntity> Added, List<ISyncEntity> Removed) Calculate(IEnumerable<ISyncEntity> current,IEnumerable<ISyncEntity> snapshot)
    {
        var added = current.Except(snapshot, new RowIdentityComparer()).ToList();
        var removed = snapshot.Except(current, new RowIdentityComparer()).ToList();

        return (added, removed);
    }
}

public class RowIdentityComparer : IEqualityComparer<ISyncEntity>
{
    public bool Equals(ISyncEntity? x, ISyncEntity? y) => x != null && y != null && x.RowIdentity == y.RowIdentity;

    public int GetHashCode(ISyncEntity obj) => obj.RowIdentity.GetHashCode();
}
