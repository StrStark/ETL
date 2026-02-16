using ETL.Models;
using System.ComponentModel.DataAnnotations;

namespace ETL.Entity;

public class HesabSummaryModel : ISyncEntity
{
    [Key]
    public string? Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Bedehkar { get; set; }
    public string? Bestankar { get; set; }
    public string RowIdentity { get; set; } = default!;
    public string SyncKey { get; set; } = default!;
    public bool IsSynced { get; set; }
    public DateTime? SyncedAt { get; set; }
}