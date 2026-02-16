namespace ETL.Models;

public record ChangeAck
{
    public Guid BatchId { get; init; }
    public List<RecordAck> Results { get; init; } = new();

}
public record RecordAck
{
    public string RowIdentity { get; init; } = default!;
    public SyncStatus Status { get; init; }
    public string? Reason { get; init; }
}
