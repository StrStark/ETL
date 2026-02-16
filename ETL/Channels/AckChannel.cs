
using ETL.Models;
using System.Threading.Channels;

namespace ETL.Channels;

public class SyncChannels
{
    public Channel<ISyncEntity> Accepted { get; } = Channel.CreateUnbounded<ISyncEntity>();
    public Channel<ISyncEntity> Rejected { get; } = Channel.CreateUnbounded<ISyncEntity>();
    public Channel<ISyncEntity> Skipped { get; } = Channel.CreateUnbounded<ISyncEntity>();
}
