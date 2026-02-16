using ETL.Models;

namespace ETL.WebSocket;

public interface IChangeSender
{
    Task SendAsync(
        string entityName,
        IReadOnlyList<ISyncEntity> added,
        IReadOnlyList<ISyncEntity> removed,
        CancellationToken ct);
}
