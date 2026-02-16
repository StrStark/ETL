using System.Text.Json;

namespace ETL.Models;

public record ChangeBatch
{
    public Guid BatchId { get; init; }
    public string Entity { get; init; } = default!;
    public JsonElement Added { get; init; } = new();
    public JsonElement Removed { get; init; } = new();
}
