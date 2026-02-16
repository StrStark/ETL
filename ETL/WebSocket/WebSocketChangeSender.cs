using ETL.Channels;
using ETL.Entity;
using ETL.Models;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace ETL.WebSocket;

public class WebSocketChangeSender( WebSocketManager ws, SyncChannels channels) : IChangeSender
{

    public async Task SendAsync(string entityName,IReadOnlyList<ISyncEntity> added,IReadOnlyList<ISyncEntity> removed,CancellationToken ct)
    {
        const int batchSize = 1000;
        
        var addedChunks = Chunk(added, batchSize);
        var removedChunks = Chunk(removed, batchSize);

        var socket = ws.Socket;
        if (ws.Socket.State != WebSocketState.Open)
        {
            await ws.StartAsync(ct);

        }
        using var addedEnum = addedChunks.GetEnumerator();
        using var removedEnum = removedChunks.GetEnumerator();

        while (true)
        {
            var hasAdded = addedEnum.MoveNext();
            var hasRemoved = removedEnum.MoveNext();

            if (!hasAdded && !hasRemoved)
                break;

            var batch = new ChangeBatch
            {
                BatchId = Guid.NewGuid(),
                Entity = entityName,
                Added = hasAdded
                ? entityName switch
                {
                    "sales" => JsonSerializer.SerializeToElement(addedEnum.Current.Cast<SaleRecordModel>().ToList()),
                    "sale_returns" => JsonSerializer.SerializeToElement(addedEnum.Current.Cast<SaleReturnRecordModel>().ToList()),
                    "counterparties" => JsonSerializer.SerializeToElement( addedEnum.Current.Cast<CounterPartyModel>().ToList()),
                    "financial" => JsonSerializer.SerializeToElement( addedEnum.Current.Cast<HesabSummaryModel>().ToList()),
                    "product" => JsonSerializer.SerializeToElement(addedEnum.Current.Cast<AnbarResidHavaleDetailModel>().ToList()),
                    "salesreturn" => JsonSerializer.SerializeToElement(addedEnum.Current.Cast<SaleReturnRecordModel>().ToList()),
                    _ => JsonSerializer.SerializeToElement(addedEnum.Current)
                }
                : JsonSerializer.SerializeToElement(new List<ISyncEntity>()),
                
                Removed = hasRemoved
                ? entityName switch
                {
                    "sales" => JsonSerializer.SerializeToElement(addedEnum.Current.Cast<SaleRecordModel>().ToList()),
                    "sale_returns" => JsonSerializer.SerializeToElement(addedEnum.Current.Cast<SaleReturnRecordModel>().ToList()),
                    "counterparties" => JsonSerializer.SerializeToElement(addedEnum.Current.Cast<CounterPartyModel>().ToList()),
                    "financial" => JsonSerializer.SerializeToElement(addedEnum.Current.Cast<HesabSummaryModel>().ToList()),
                    "product" => JsonSerializer.SerializeToElement(addedEnum.Current.Cast<AnbarResidHavaleDetailModel>().ToList()),
                    "salesreturn" => JsonSerializer.SerializeToElement(addedEnum.Current.Cast<SaleReturnRecordModel>().ToList()),
                    _ => JsonSerializer.SerializeToElement(addedEnum.Current)
                }
                : JsonSerializer.SerializeToElement(new List<ISyncEntity>()),
            };

            await SendBatchAsync(socket, batch, ct);
        }
        await ws.StopAsync(ct);
    }

    private async Task SendBatchAsync(ClientWebSocket socket, ChangeBatch batch,CancellationToken ct)
    {
            await socket.SendAsync(
            JsonSerializer.SerializeToUtf8Bytes(batch),
            WebSocketMessageType.Text,
            true,
            ct);

        var ackJson = await ReceiveFullMessage(socket, ct);


        var ack = JsonSerializer.Deserialize<ChangeAck>(ackJson)!;




        var addedIndex = (batch.Entity switch
        {
            "sales" => batch.Added.Deserialize<List<SaleRecordModel>>()!.Cast<ISyncEntity>().ToList(),
            "sale_returns" => batch.Added.Deserialize<List<SaleReturnRecordModel>>()!.Cast<ISyncEntity>().ToList(),
            "counterparties" => batch.Added.Deserialize<List<CounterPartyModel>>()!.Cast<ISyncEntity>().ToList(),
            "financial" => batch.Added.Deserialize<List<HesabSummaryModel>>()!.Cast<ISyncEntity>().ToList(),
            "product" => batch.Added.Deserialize<List<AnbarResidHavaleDetailModel>>()!.Cast<ISyncEntity>().ToList(),
            "salesreturn" => batch.Added.Deserialize<List<SaleReturnRecordModel>>()!.Cast<ISyncEntity>().ToList(),
            _ => batch.Added.Deserialize<List<ISyncEntity>>()!
        }).ToDictionary(x => x.RowIdentity); // lets generate an id for the ack seding and reciving .... :)))))))))))))))))))))))))))))))))))))))

        foreach (var r in ack.Results   )
        {
            if (!addedIndex.TryGetValue(r.RowIdentity, out var entity))
                continue;

            switch (r.Status)
            {
                case SyncStatus.Accepted:
                    await channels.Accepted.Writer.WriteAsync(entity, ct);
                    break;

                case SyncStatus.Rejected:
                    await channels.Rejected.Writer.WriteAsync(entity, ct);
                    break;

                case SyncStatus.Skipped:
                    await channels.Skipped.Writer.WriteAsync(entity, ct);
                    break;
            }
        }
    }
    private static IEnumerable<List<T>> Chunk<T>(IReadOnlyList<T> source, int size)
    {
        for (int i = 0; i < source.Count; i += size)
            yield return source.Skip(i).Take(size).ToList();
    }
    private static async Task<string> ReceiveFullMessage(ClientWebSocket socket, CancellationToken ct)
    {
        var buffer = new byte[32 * 1024];
        using var ms = new MemoryStream();

        while (true)
        {
            var result = await socket.ReceiveAsync(buffer, ct);

            if (result.MessageType == WebSocketMessageType.Close)
                throw new WebSocketException("Socket closed");

            ms.Write(buffer, 0, result.Count);

            if (result.EndOfMessage)
                break;
        }

        return Encoding.UTF8.GetString(ms.ToArray());
    }


}
