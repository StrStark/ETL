using ETL.Models;
using System.ComponentModel.DataAnnotations;

namespace ETL.Entity;

public class CounterPartyModel : ISyncEntity
{
    [Key]
    public Guid Id { get; set; }
    public string? HesabID { get; set; }
    public string? CounterPartyCode { get; set; }
    public string? FullName { get; set; }

    public string? HesabAnbar { get; set; }
    public string? CustomerType { get; set; }

    public string? CustomerName { get; set; }
    public string? CusotmerFamily { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerIdentiifre { get; set; }

    public string? CustomerTel { get; set; }
    public string? CustomerPhoneNumber { get; set; }
    public string? CustomerPelak { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerBirthDay { get; set; }
    public string? Active { get; set; }

    public string RowIdentity { get; set; } = default!;
    public string SyncKey { get; set; } = default!;
    public bool IsSynced { get; set; }
    public DateTime? SyncedAt { get; set; }
}
