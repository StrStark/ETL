using ETL.Models;
using System.ComponentModel.DataAnnotations;

namespace ETL.Entity;

public class SaleReturnRecordModel : ISyncEntity
{
    [Key]
    public Guid Id { get; set; }
    public string? SandID { get; set; }
    public string? DocNum { get; set; }
    public string? AccountCode { get; set; }
    public string? Article { get; set; }
    public string? HavaleCode { get; set; }
    public string? Type { get; set; }
    public string? Date { get; set; }

    public string? KalaCode { get; set; }
    public string? KalDelimiter { get; set; }
    public string? KDesc { get; set; }

    public string? UnitCode1 { get; set; }
    public string? UnitValue1 { get; set; }
    public string? UnitCode2 { get; set; }
    public string? UnitValue2 { get; set; }
    public string? UnitCode3 { get; set; }
    public string? UnitValue3 { get; set; }

    public string? Price { get; set; }
    public string? TarafHesab { get; set; }

    public string? CustomerType { get; set; }
    public string? CustomerAccount { get; set; }
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

    public string? DetailedType { get; set; }
    public string RowIdentity { get; set; } = default!;
    public string SyncKey { get; set; } = default!;
    public bool IsSynced { get; set; }
    public DateTime? SyncedAt { get; set; }
}
