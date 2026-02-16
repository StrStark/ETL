using ETL.Models;

namespace ETL.Services;

public interface IEntitySyncService
{
    string EntityName { get; }

    Task<IReadOnlyList<ISyncEntity>> LoadCurrentAsync(CancellationToken ct);
    Task<IReadOnlyList<ISyncEntity>> LoadSnapshotAsync(CancellationToken ct);
    Task ClearSnapshot(CancellationToken ct);

    Task SaveSnapshotAsync(
        IEnumerable<ISyncEntity> snapshot,
        CancellationToken ct);
}
